using System;
using System.Collections;
using System.Linq;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using UnityEngine.TestTools.Logging;
using UnityEngine.TestTools.TestRunner;

namespace UnityEngine.TestRunner.NUnitExtensions.Runner
{
    internal class UnityLogCheckDelegatingCommand : DelegatingTestCommand, IEnumerableTestMethodCommand
    {
        public UnityLogCheckDelegatingCommand(TestCommand innerCommand)
            : base(innerCommand) {}

        public override TestResult Execute(ITestExecutionContext context)
        {
            var logCollector = new LogScope();

            try
            {
                innerCommand.Execute(context);

                if (logCollector.AnyFailingLogs())
                {
                    var failingLog = logCollector.FailingLogs.First();
                    throw new UnhandledLogMessageException(failingLog);
                }

                if (logCollector.ExpectedLogs.Any())
                {
                    throw new UnexpectedLogMessageException(logCollector.ExpectedLogs.Peek());
                }
            }
            catch (Exception exception)
            {
                context.CurrentResult.RecordException(exception);
            }
            logCollector.Dispose();
            return context.CurrentResult;
        }

        public IEnumerable ExecuteEnumerable(ITestExecutionContext context)
        {
            var logCollector = new LogScope();

            if (!(innerCommand is IEnumerableTestMethodCommand))
            {
                Execute(context);
                yield break;
            }

            var enumerableTestMethodCommand = (IEnumerableTestMethodCommand)innerCommand;

            IEnumerable executeEnumerable;

            try
            {
                executeEnumerable = enumerableTestMethodCommand.ExecuteEnumerable(context);
            }
            catch (Exception e)
            {
                context.CurrentResult.RecordException(e);
                yield break;
            }

            foreach (var step in executeEnumerable)
            {
                try
                {
                    if (logCollector.AnyFailingLogs())
                    {
                        var failingLog = logCollector.FailingLogs.First();
                        throw new UnhandledLogMessageException(failingLog);
                    }
                }
                catch (Exception e)
                {
                    context.CurrentResult.RecordException(e);
                    break;
                }
                yield return step;
            }

            try
            {
                if (logCollector.AnyFailingLogs())
                {
                    var failingLog = logCollector.FailingLogs.First();
                    throw new UnhandledLogMessageException(failingLog);
                }

                logCollector.ProcessExpectedLogs();
                if (logCollector.ExpectedLogs.Any())
                {
                    throw new UnexpectedLogMessageException(LogScope.Current.ExpectedLogs.Peek());
                }
            }
            catch (Exception exception)
            {
                context.CurrentResult.RecordException(exception);
            }
            logCollector.Dispose();
        }
    }
}
