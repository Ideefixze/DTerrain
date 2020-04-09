using System;
using UnityEngine;
using UnityEngine.TestTools.TestRunner.GUI;

namespace UnityEditor.TestTools.TestRunner.Api
{
    [Serializable]
    internal class Filter
    {
        [SerializeField]
        public TestMode testMode;
        [SerializeField]
        public string[] testNames;
        [SerializeField]
        public string[] groupNames;
        [SerializeField]
        public string[] categoryNames;
        [SerializeField]
        public string[] assemblyNames;

        public static Filter empty = new Filter();

        internal TestRunnerFilter ToTestRunnerFilter()
        {
            return new TestRunnerFilter()
            {
                testNames = testNames,
                categoryNames = categoryNames,
                groupNames = groupNames,
                assemblyNames = assemblyNames
            };
        }
    }
}
