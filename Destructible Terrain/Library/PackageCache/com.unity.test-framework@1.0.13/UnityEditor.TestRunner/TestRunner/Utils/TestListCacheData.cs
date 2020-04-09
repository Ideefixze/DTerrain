using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestRunner.TestLaunchers;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner
{
    internal class TestListCacheData : ScriptableSingleton<TestListCacheData>, ITestListCacheData
    {
        [SerializeField]
        private List<TestPlatform> m_Platforms = new List<TestPlatform>();

        [SerializeField]
        private List<RemoteTestResultDataWithTestData> m_CachedData = new List<RemoteTestResultDataWithTestData>();

        public List<TestPlatform> platforms
        {
            get { return m_Platforms; }
        }

        public List<RemoteTestResultDataWithTestData> cachedData
        {
            get { return m_CachedData; }
        }
    }
}
