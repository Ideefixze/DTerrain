using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.TestRunner.TestLaunchers;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.TestRunner;
using UnityEngine.TestTools.TestRunner.Callbacks;

namespace UnityEditor.TestTools.TestRunner
{
    internal class TestLaunchFailedException : Exception
    {
        public TestLaunchFailedException() {}
        public TestLaunchFailedException(string message) : base(message) {}
    }

    [Serializable]
    internal class PlayerLauncher : RuntimeTestLauncherBase
    {
        private readonly PlaymodeTestsControllerSettings m_Settings;
        private readonly BuildTarget m_TargetPlatform;
        private string m_TempBuildLocation;
        private ITestRunSettings m_OverloadTestRunSettings;

        public PlayerLauncher(PlaymodeTestsControllerSettings settings, BuildTarget? targetPlatform, ITestRunSettings overloadTestRunSettings)
        {
            m_Settings = settings;
            m_TargetPlatform = targetPlatform ?? EditorUserBuildSettings.activeBuildTarget;
            m_OverloadTestRunSettings = overloadTestRunSettings;
        }

        protected override RuntimePlatform? TestTargetPlatform
        {
            get { return BuildTargetConverter.TryConvertToRuntimePlatform(m_TargetPlatform); }
        }

        public override void Run()
        {
            var editorConnectionTestCollector = RemoteTestRunController.instance;
            editorConnectionTestCollector.hideFlags = HideFlags.HideAndDontSave;
            editorConnectionTestCollector.Init(m_TargetPlatform);

            var remotePlayerLogController = RemotePlayerLogController.instance;
            remotePlayerLogController.hideFlags = HideFlags.HideAndDontSave;

            using (var settings = new PlayerLauncherContextSettings(m_OverloadTestRunSettings))
            {
                var sceneName = CreateSceneName();
                var scene = PrepareScene(sceneName);

                var filter = m_Settings.filter.BuildNUnitFilter();
                var runner = LoadTests(filter);
                var exceptionThrown = ExecutePreBuildSetupMethods(runner.LoadedTest, filter);
                if (exceptionThrown)
                {
                    ReopenOriginalScene(m_Settings.originalScene);
                    AssetDatabase.DeleteAsset(sceneName);
                    CallbacksDelegator.instance.RunFailed("Run Failed: One or more errors in a prebuild setup. See the editor log for details.");
                    return;
                }

                var playerBuildOptions = GetBuildOptions(scene);

                var success = BuildAndRunPlayer(playerBuildOptions);
                editorConnectionTestCollector.PostBuildAction();
                ExecutePostBuildCleanupMethods(runner.LoadedTest, filter);

                ReopenOriginalScene(m_Settings.originalScene);
                AssetDatabase.DeleteAsset(sceneName);

                if (!success)
                {
                    ScriptableObject.DestroyImmediate(editorConnectionTestCollector);
                    Debug.LogError("Player build failed");
                    throw new TestLaunchFailedException("Player build failed");
                }

                editorConnectionTestCollector.PostSuccessfulBuildAction();
            }
        }

        public Scene PrepareScene(string sceneName)
        {
            var scene = CreateBootstrapScene(sceneName, runner =>
            {
                runner.AddEventHandlerMonoBehaviour<PlayModeRunnerCallback>();
                runner.settings = m_Settings;
                runner.AddEventHandlerMonoBehaviour<RemoteTestResultSender>();
            });
            return scene;
        }

        private static bool BuildAndRunPlayer(PlayerLauncherBuildOptions buildOptions)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Building player with following options:\n{0}", buildOptions);


            // Android has to be in listen mode to establish player connection
            if (buildOptions.BuildPlayerOptions.target == BuildTarget.Android)
            {
                buildOptions.BuildPlayerOptions.options &= ~BuildOptions.ConnectToHost;
            }

            // For now, so does Lumin
            if (buildOptions.BuildPlayerOptions.target == BuildTarget.Lumin)
            {
                buildOptions.BuildPlayerOptions.options &= ~BuildOptions.ConnectToHost;
            }

            var result = BuildPipeline.BuildPlayer(buildOptions.BuildPlayerOptions);
            if (result.summary.result != Build.Reporting.BuildResult.Succeeded)
                Debug.LogError(result.SummarizeErrors());

            return result.summary.result == Build.Reporting.BuildResult.Succeeded;
        }

        private PlayerLauncherBuildOptions GetBuildOptions(Scene scene)
        {
            var buildOptions = new BuildPlayerOptions();
            var reduceBuildLocationPathLength = false;

            //Some platforms hit MAX_PATH limits during the build process, in these cases minimize the path length
            if ((m_TargetPlatform == BuildTarget.WSAPlayer) || (m_TargetPlatform == BuildTarget.XboxOne))
            {
                reduceBuildLocationPathLength = true;
            }

            var scenes = new List<string>() { scene.path };
            scenes.AddRange(EditorBuildSettings.scenes.Select(x => x.path));
            buildOptions.scenes = scenes.ToArray();

            buildOptions.options |= BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.ConnectToHost | BuildOptions.IncludeTestAssemblies | BuildOptions.StrictMode;
            buildOptions.target = m_TargetPlatform;

            if (EditorUserBuildSettings.waitForPlayerConnection)
                buildOptions.options |= BuildOptions.WaitForPlayerConnection;

            var buildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
            var uniqueTempPathInProject = FileUtil.GetUniqueTempPathInProject();

            if (reduceBuildLocationPathLength)
            {
                uniqueTempPathInProject = Path.GetTempFileName();
                File.Delete(uniqueTempPathInProject);
                Directory.CreateDirectory(uniqueTempPathInProject);
            }

            //Check if Lz4 is supported for the current buildtargetgroup and enable it if need be
            if (PostprocessBuildPlayer.SupportsLz4Compression(buildTargetGroup, m_TargetPlatform))
            {
                if (EditorUserBuildSettings.GetCompressionType(buildTargetGroup) == Compression.Lz4)
                    buildOptions.options |= BuildOptions.CompressWithLz4;
                else if (EditorUserBuildSettings.GetCompressionType(buildTargetGroup) == Compression.Lz4HC)
                    buildOptions.options |= BuildOptions.CompressWithLz4HC;
            }

            m_TempBuildLocation = Path.GetFullPath(uniqueTempPathInProject);

            string extensionForBuildTarget = PostprocessBuildPlayer.GetExtensionForBuildTarget(buildTargetGroup, buildOptions.target, buildOptions.options);

            var playerExecutableName = "PlayerWithTests";
            var playerDirectoryName = reduceBuildLocationPathLength ? "PwT" : "PlayerWithTests";

            var locationPath = Path.Combine(m_TempBuildLocation, playerDirectoryName);

            if (!string.IsNullOrEmpty(extensionForBuildTarget))
            {
                playerExecutableName += string.Format(".{0}", extensionForBuildTarget);
                locationPath = Path.Combine(locationPath, playerExecutableName);
            }

            buildOptions.locationPathName = locationPath;

            return new PlayerLauncherBuildOptions
            {
                BuildPlayerOptions = buildOptions,
                PlayerDirectory = Path.Combine(m_TempBuildLocation, playerDirectoryName),
            };
        }
    }
}
