using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools.TestRunner.GUI;

namespace UnityEditor.TestTools.TestRunner
{
    internal class RerunCallbackData : ScriptableSingleton<RerunCallbackData>
    {
        [SerializeField]
        internal TestRunnerFilter runFilter;

        [SerializeField]
        internal TestMode testMode;
    }
}
