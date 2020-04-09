using System;
using UnityEngine.TestTools;
using UnityEngine.TestTools.TestRunner;

namespace UnityEditor.TestTools.TestRunner.Api
{
    internal class TestLauncherFactory
    {
        internal TestLauncherBase GetLauncher(ExecutionSettings executionSettings)
        {
            if (executionSettings.filter.testMode == TestMode.EditMode)
            {
                return GetEditModeLauncher(executionSettings.filter);
            }
            else
            {
                var settings = PlaymodeTestsControllerSettings.CreateRunnerSettings(executionSettings.filter.ToTestRunnerFilter());
                return GetPlayModeLauncher(settings, executionSettings);
            }
        }

        static TestLauncherBase GetEditModeLauncher(Filter filter)
        {
            return GetEditModeLauncherForProvidedAssemblies(filter);
        }

        static TestLauncherBase GetPlayModeLauncher(PlaymodeTestsControllerSettings settings, ExecutionSettings executionSettings)
        {
            if (executionSettings.targetPlatform != null)
            {
                return GetPlayerLauncher(settings, executionSettings.targetPlatform.Value, executionSettings.overloadTestRunSettings);
            }

            if (PlayerSettings.runPlayModeTestAsEditModeTest)
            {
                return GetEditModeLauncherForProvidedAssemblies(executionSettings.filter, TestPlatform.PlayMode);
            }

            return GetPlayModeLauncher(settings);
        }

        static TestLauncherBase GetEditModeLauncherForProvidedAssemblies(Filter filter, TestPlatform testPlatform = TestPlatform.EditMode)
        {
            return new EditModeLauncher(filter.ToTestRunnerFilter(), testPlatform);
        }

        static TestLauncherBase GetPlayModeLauncher(PlaymodeTestsControllerSettings settings)
        {
            return new PlaymodeLauncher(settings);
        }

        static TestLauncherBase GetPlayerLauncher(PlaymodeTestsControllerSettings settings, BuildTarget targetPlatform, ITestRunSettings overloadTestRunSettings)
        {
            return new PlayerLauncher(settings, targetPlatform, overloadTestRunSettings);
        }
    }
}
