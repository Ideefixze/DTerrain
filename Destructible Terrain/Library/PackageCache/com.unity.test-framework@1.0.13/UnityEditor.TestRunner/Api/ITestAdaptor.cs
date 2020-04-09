using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace UnityEditor.TestTools.TestRunner.Api
{
    internal interface ITestAdaptor
    {
        string Id { get; }
        string Name { get; }
        string FullName { get; }
        int TestCaseCount { get; }
        bool HasChildren { get; }
        bool IsSuite { get; }
        IEnumerable<ITestAdaptor> Children { get; }
        int TestCaseTimeout { get; }
        ITypeInfo TypeInfo { get; }
        IMethodInfo Method { get; }
        string[] Categories { get; }
        bool IsTestAssembly { get; }
        RunState RunState { get; }
        string Description { get; }
        string SkipReason { get; }
        string ParentId { get; }
        string UniqueName { get; }
        string ParentUniqueName { get; }
    }
}
