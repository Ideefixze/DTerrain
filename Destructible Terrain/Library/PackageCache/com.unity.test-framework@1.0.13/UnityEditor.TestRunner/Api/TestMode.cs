using System;

namespace UnityEditor.TestTools.TestRunner.Api
{
    [Flags]
    internal enum TestMode
    {
        EditMode = 1 << 0,
        PlayMode = 1 << 1
    }
}
