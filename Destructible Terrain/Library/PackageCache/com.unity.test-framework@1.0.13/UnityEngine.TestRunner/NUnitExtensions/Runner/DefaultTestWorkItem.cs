using System;
using System.Collections;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;
using UnityEngine.TestTools;
using SetUpTearDownCommand = NUnit.Framework.Internal.Commands.SetUpTearDownCommand;
using TestActionCommand = NUnit.Framework.Internal.Commands.TestActionCommand;

namespace UnityEngine.TestRunner.NUnitExtensions.Runner
{
    internal class EditModeTestCallbacks
    {
        public static Action RestoringTestContext { get; set; }
    }

    internal class DefaultTestWorkItem : UnityWorkItem
    {
        private TestCommand _command;

        public DefaultTestWorkItem(TestMethod test, ITestFilter filter)
            : base(test, null)
        {
            _command = test.RunState == RunState.Runnable || test.RunState == RunState.Explicit && filter.IsExplicitMatch(test)
                ? BuildTestCommand(test)
                : new SkipCommand(test);
        }

        private static TestCommand BuildTestCommand(TestMethod test)
        {
            var command = (TestCommand)new TestMethodCommand(test);
            command = new UnityLogCheckDelegatingCommand(command);
            foreach (var wrapper in test.Method.GetCustomAttributes<IWrapTestMethod>(true))
            {
                command = wrapper.Wrap(command);
                if (command == null)
                {
                    var message = string.Format("IWrapTestMethod implementation '{0}' returned null as command.", wrapper.GetType().FullName);
                    return new FailCommand(test, ResultState.Failure, message);
                }
            }

            command = new TestTools.TestActionCommand(command);
            command = new TestTools.SetUpTearDownCommand(command);
            command = new ImmediateEnumerableCommand(command);
            foreach (var wrapper in test.Method.GetCustomAttributes<IWrapSetUpTearDown>(true))
            {
                command = wrapper.Wrap(command);
                if (command == null)
                {
                    var message = string.Format("IWrapSetUpTearDown implementation '{0}' returned null as command.", wrapper.GetType().FullName);
                    return new FailCommand(test, ResultState.Failure, message);
                }
            }

            command = new EnumerableSetUpTearDownCommand(command);
            command = new OuterUnityTestActionCommand(command);

            IApplyToContext[] changes = test.Method.GetCustomAttributes<IApplyToContext>(true);
            if (changes.Length > 0)
            {
                command = new EnumerableApplyChangesToContextCommand(command, changes);
            }

            return command;
        }

        protected override IEnumerable PerformWork()
        {
            if (m_DontRunRestoringResult && EditModeTestCallbacks.RestoringTestContext != null)
            {
                EditModeTestCallbacks.RestoringTestContext();
                Result = Context.CurrentResult;
                yield break;
            }

            try
            {
                if (_command is SkipCommand || _command is FailCommand)
                {
                    Result = _command.Execute(Context);
                    yield break;
                }

                if (!(_command is IEnumerableTestMethodCommand))
                {
                    Debug.LogError("Cannot perform work on " + _command.GetType().Name);
                    yield break;
                }

                foreach (var workItemStep in ((IEnumerableTestMethodCommand)_command).ExecuteEnumerable(Context))
                {
                    ResultedInDomainReload = false;

                    if (workItemStep is IEditModeTestYieldInstruction)
                    {
                        var editModeTestYieldInstruction = (IEditModeTestYieldInstruction)workItemStep;
                        yield return editModeTestYieldInstruction;
                        var enumerator = editModeTestYieldInstruction.Perform();
                        while (true)
                        {
                            bool moveNext;
                            try
                            {
                                moveNext = enumerator.MoveNext();
                            }
                            catch (Exception e)
                            {
                                Context.CurrentResult.RecordException(e);
                                break;
                            }

                            if (!moveNext)
                            {
                                break;
                            }

                            yield return null;
                        }
                    }
                    else
                    {
                        yield return workItemStep;
                    }
                }

                Result = Context.CurrentResult;
            }
            finally
            {
                WorkItemComplete();
            }
        }
    }
}
