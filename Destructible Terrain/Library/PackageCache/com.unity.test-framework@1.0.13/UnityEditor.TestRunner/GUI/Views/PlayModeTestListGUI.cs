using System;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.TestRunner;
using UnityEngine.TestTools.TestRunner.GUI;

namespace UnityEditor.TestTools.TestRunner.GUI
{
    [Serializable]
    internal class PlayModeTestListGUI : TestListGUI
    {
        public override TestMode TestMode
        {
            get { return TestMode.PlayMode; }
        }
        public override void PrintHeadPanel()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
            base.PrintHeadPanel();
            if (GUILayout.Button("Run all in player (" + EditorUserBuildSettings.activeBuildTarget + ")", EditorStyles.toolbarButton))
            {
                RunTestsInPlayer(null);
            }
            EditorGUILayout.EndHorizontal();
            DrawFilters();
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
            EditorGUILayout.EndHorizontal();
        }

        public override void RenderNoTestsInfo()
        {
            if (!TestListGUIHelper.SelectedFolderContainsTestAssembly())
            {
                var noTestText = "No tests to show";
                if (!PlayerSettings.playModeTestRunnerEnabled)
                {
                    const string testsArePulledFromCustomAssemblues = "Test Assemblies are defined by Assembly Definitions where you add Unity References \"Test Assemblies\"";
                    const string infoTextAboutTestsInAllAssemblies =
                        "To have tests in all assemblies enable it in the Test Runner window context menu";
                    noTestText += Environment.NewLine + testsArePulledFromCustomAssemblues + Environment.NewLine +
                        infoTextAboutTestsInAllAssemblies;
                }

                EditorGUILayout.HelpBox(noTestText, MessageType.Info);
                if (GUILayout.Button("Create PlayMode Test Assembly Folder"))
                {
                    TestListGUIHelper.AddFolderAndAsmDefForTesting();
                }
            }

            if (!TestListGUIHelper.CanAddPlayModeTestScriptAndItWillCompile())
            {
                UnityEngine.GUI.enabled = false;
                EditorGUILayout.HelpBox("PlayMode test scripts can only be created in non editor test assemblies.", MessageType.Warning);
            }
            if (GUILayout.Button("Create Test Script in current folder"))
            {
                TestListGUIHelper.AddTest();
            }
            UnityEngine.GUI.enabled = true;
        }

        protected override void RunTests(TestRunnerFilter filter)
        {
            // Give user chance to save the changes to their currently open scene because we close it and load our own
            var cancelled = !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (cancelled)
                return;

            filter.ClearResults(newResultList.OfType<TestRunnerFilter.IClearableResult>().ToList());

            RerunCallbackData.instance.runFilter = filter;
            RerunCallbackData.instance.testMode = TestMode.PlayMode;

            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.Execute(new ExecutionSettings()
            {
                filter = new Filter()
                {
                    categoryNames = filter.categoryNames,
                    groupNames =  filter.groupNames,
                    testMode = TestMode,
                    testNames = filter.testNames
                }
            });
        }

        protected void RunTestsInPlayer(TestRunnerFilter filter)
        {
            var settings = PlaymodeTestsControllerSettings.CreateRunnerSettings(filter);
            var testExecutor = new PlayerLauncher(settings, null, null);
            testExecutor.Run();
            GUIUtility.ExitGUI();
        }

        public override TestPlatform TestPlatform { get { return TestPlatform.PlayMode; } }
        protected override bool IsBusy()
        {
            return PlaymodeLauncher.IsRunning  || EditorApplication.isCompiling || EditorApplication.isPlaying;
        }
    }
}
