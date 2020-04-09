namespace UnityEditor.TestTools.TestRunner.Api
{
    internal class ExecutionSettings
    {
        public BuildTarget? targetPlatform;
        public ITestRunSettings overloadTestRunSettings;
        public Filter filter;
    }
}
