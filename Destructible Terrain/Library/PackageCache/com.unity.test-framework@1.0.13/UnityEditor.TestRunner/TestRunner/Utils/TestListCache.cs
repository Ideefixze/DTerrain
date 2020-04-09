using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine.TestRunner.TestLaunchers;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner
{
    internal class TestListCache : ITestListCache
    {
        private readonly ITestAdaptorFactory m_TestAdaptorFactory;
        private readonly IRemoteTestResultDataFactory m_TestResultDataFactory;
        private readonly ITestListCacheData m_TestListCacheData;

        public TestListCache(ITestAdaptorFactory testAdaptorFactory, IRemoteTestResultDataFactory testResultDataFactory, ITestListCacheData testListCacheData)
        {
            m_TestAdaptorFactory = testAdaptorFactory;
            m_TestResultDataFactory = testResultDataFactory;
            m_TestListCacheData = testListCacheData;
        }

        public void CacheTest(TestPlatform platform, ITest test)
        {
            var data = m_TestResultDataFactory.CreateFromTest(test);

            var index = m_TestListCacheData.platforms.IndexOf(platform);
            if (index < 0)
            {
                m_TestListCacheData.cachedData.Add(data);
                m_TestListCacheData.platforms.Add(platform);
            }
            else
            {
                m_TestListCacheData.cachedData[index] = data;
            }
        }

        public IEnumerator<ITestAdaptor> GetTestFromCacheAsync(TestPlatform platform)
        {
            var index = m_TestListCacheData.platforms.IndexOf(platform);
            if (index < 0)
            {
                yield return null;
                yield break;
            }

            var testData = m_TestListCacheData.cachedData[index];
            var test = m_TestAdaptorFactory.BuildTreeAsync(testData);
            while (test.MoveNext())
            {
                yield return null;
            }

            yield return test.Current;
        }

        [Callbacks.DidReloadScripts]
        private static void ScriptReloaded()
        {
            TestListCacheData.instance.cachedData.Clear();
            TestListCacheData.instance.platforms.Clear();
        }
    }
}
