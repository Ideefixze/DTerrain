using System.Collections.Generic;
using UnityEngine.TestRunner.TestLaunchers;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner
{
    interface ITestListCacheData
    {
        List<TestPlatform> platforms { get; }
        List<RemoteTestResultDataWithTestData> cachedData { get; }
    }
}
