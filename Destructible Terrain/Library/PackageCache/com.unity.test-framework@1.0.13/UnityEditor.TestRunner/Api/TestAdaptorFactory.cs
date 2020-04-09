using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using UnityEngine.TestRunner.TestLaunchers;

namespace UnityEditor.TestTools.TestRunner.Api
{
    internal class TestAdaptorFactory : ITestAdaptorFactory
    {
        public ITestAdaptor Create(ITest test)
        {
            return new TestAdaptor(test);
        }

        public ITestAdaptor Create(RemoteTestData testData)
        {
            return new TestAdaptor(testData);
        }

        public ITestResultAdaptor Create(ITestResult testResult)
        {
            return new TestResultAdaptor(testResult);
        }

        public ITestResultAdaptor Create(RemoteTestResultData testResult, RemoteTestResultDataWithTestData allData)
        {
            return new TestResultAdaptor(testResult, allData);
        }

        public ITestAdaptor BuildTree(RemoteTestResultDataWithTestData data)
        {
            var tests = data.tests.Select(remoteTestData => new TestAdaptor(remoteTestData)).ToList();

            foreach (var test in tests)
            {
                test.ApplyChildren(tests);
            }

            return tests.First();
        }

        public IEnumerator<ITestAdaptor> BuildTreeAsync(RemoteTestResultDataWithTestData data)
        {
            var tests = data.tests.Select(remoteTestData => new TestAdaptor(remoteTestData)).ToList();

            for (var index = 0; index < tests.Count; index++)
            {
                var test = tests[index];
                test.ApplyChildren(tests);
                if (index % 100 == 0)
                {
                    yield return null;
                }
            }

            yield return tests.First();
        }
    }
}
