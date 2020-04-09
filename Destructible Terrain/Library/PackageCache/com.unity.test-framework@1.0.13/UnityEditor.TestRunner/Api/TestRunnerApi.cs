using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.TestRunner.TestLaunchers;
using UnityEngine.TestTools;
using UnityEngine.TestTools.NUnitExtensions;

namespace UnityEditor.TestTools.TestRunner.Api
{
    internal class TestRunnerApi : ScriptableObject, ITestRunnerApi
    {
        public void Execute(ExecutionSettings executionSettings)
        {
            if (executionSettings == null)
            {
                throw new ArgumentException("Filter for execution is undefined.");
            }

            var launcherFactory = new TestLauncherFactory();
            var data = TestRunData.instance;
            data.executionSettings = executionSettings;

            var testLauncher = launcherFactory.GetLauncher(executionSettings);
            testLauncher.Run();
        }

        public void RegisterCallbacks<T>(T testCallbacks, int priority = 0) where T : ICallbacks
        {
            if (testCallbacks == null)
            {
                throw new ArgumentException("TestCallbacks for execution is undefined.");
            }

            CallbacksHolder.instance.Add(testCallbacks, priority);
        }

        public void UnregisterCallbacks<T>(T testCallbacks) where T : ICallbacks
        {
            if (testCallbacks == null)
            {
                throw new ArgumentException("TestCallbacks for execution is undefined.");
            }

            CallbacksHolder.instance.Remove(testCallbacks);
        }

        public void RetrieveTestList(ExecutionSettings executionSettings, Action<ITestAdaptor> callback)
        {
            if (executionSettings == null)
            {
                throw new ArgumentException("Filter for execution is undefined.");
            }

            if (callback == null)
            {
                throw new ArgumentException("Callback is undefined.");
            }

            var platform = ParseTestMode(executionSettings.filter.testMode);
            var testAssemblyProvider = new EditorLoadedTestAssemblyProvider(new EditorCompilationInterfaceProxy(), new EditorAssembliesProxy());
            var testAdaptorFactory = new TestAdaptorFactory();
            var testListCache = new TestListCache(testAdaptorFactory, new RemoteTestResultDataFactory(), TestListCacheData.instance);
            var testListProvider = new TestListProvider(testAssemblyProvider, new UnityTestAssemblyBuilder());
            var cachedTestListProvider = new CachingTestListProvider(testListProvider, testListCache, testAdaptorFactory);

            var job = new TestListJob(cachedTestListProvider, platform, callback);
            job.Start();
        }

        private static TestPlatform ParseTestMode(TestMode testmode)
        {
            if (testmode == TestMode.EditMode)
            {
                return TestPlatform.EditMode;
            }

            return TestPlatform.PlayMode;
        }
    }
}
