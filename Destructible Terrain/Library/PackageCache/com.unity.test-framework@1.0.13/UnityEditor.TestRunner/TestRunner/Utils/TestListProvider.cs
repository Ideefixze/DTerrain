using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using UnityEngine.TestTools;
using UnityEngine.TestTools.NUnitExtensions;

namespace UnityEditor.TestTools.TestRunner
{
    internal class TestListProvider : ITestListProvider
    {
        private readonly EditorLoadedTestAssemblyProvider m_AssemblyProvider;
        private readonly UnityTestAssemblyBuilder m_AssemblyBuilder;

        public TestListProvider(EditorLoadedTestAssemblyProvider assemblyProvider, UnityTestAssemblyBuilder assemblyBuilder)
        {
            m_AssemblyProvider = assemblyProvider;
            m_AssemblyBuilder = assemblyBuilder;
        }

        public IEnumerator<ITest> GetTestListAsync(TestPlatform platform)
        {
            var assemblies = m_AssemblyProvider.GetAssembliesGroupedByTypeAsync(platform);
            while (assemblies.MoveNext())
            {
                yield return null;
            }

            var settings = UnityTestAssemblyBuilder.GetNUnitTestBuilderSettings(platform);
            var test =  m_AssemblyBuilder.BuildAsync(assemblies.Current.Select(x => x.Assembly).ToArray(), settings);
            while (test.MoveNext())
            {
                yield return null;
            }

            yield return test.Current;
        }
    }
}
