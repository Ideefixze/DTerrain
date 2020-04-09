using System;

namespace UnityEditor.TestTools.TestRunner.Api
{
    interface ITestRunnerApi
    {
        void Execute(ExecutionSettings executionSettings);
        void RegisterCallbacks<T>(T testCallbacks, int priority = 0) where T : ICallbacks;
        void UnregisterCallbacks<T>(T testCallbacks) where T : ICallbacks;
        void RetrieveTestList(ExecutionSettings executionSettings, Action<ITestAdaptor> callback);
    }
}
