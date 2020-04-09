using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace UnityEngine.TestTools.NUnitExtensions
{
    internal class UnityTestAssemblyBuilder : DefaultTestAssemblyBuilder
    {
        private readonly string m_ProductName;
        public UnityTestAssemblyBuilder()
        {
            m_ProductName = Application.productName;
        }

        public ITest Build(Assembly[] assemblies, IDictionary<string, object> options)
        {
            var test = BuildAsync(assemblies, options);
            while (test.MoveNext())
            {
            }

            return test.Current;
        }

        public IEnumerator<ITest> BuildAsync(Assembly[] assemblies, IDictionary<string, object> options)
        {
            var productName = string.Join("_", m_ProductName.Split(Path.GetInvalidFileNameChars()));
            var suite = new TestSuite(productName);
            foreach (var assembly in assemblies)
            {
                var assemblySuite = Build(assembly, options) as TestSuite;
                if (assemblySuite != null && assemblySuite.HasChildren)
                {
                    suite.Add(assemblySuite);
                }
                yield return null;
            }
            yield return suite;
        }

        public static Dictionary<string, object> GetNUnitTestBuilderSettings(TestPlatform testPlatform)
        {
            var emptySettings = new Dictionary<string, object>();
            emptySettings.Add(FrameworkPackageSettings.TestParameters, "platform=" + testPlatform);
            return emptySettings;
        }
    }
}
