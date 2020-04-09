using System;

namespace UnityEditor.TestTools.TestRunner.Api
{
    internal interface ITestRunSettings : IDisposable
    {
        void Apply();
    }
}
