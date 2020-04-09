using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.TestTools.Logging
{
    internal class LogScope : IDisposable
    {
        private bool m_Disposed;
        private readonly object _lock = new object();

        public Queue<LogMatch> ExpectedLogs { get; set; }
        public List<LogEvent> AllLogs { get; }
        public List<LogEvent> FailingLogs { get; }
        public bool IgnoreFailingMessages { get; set; }
        public bool IsNUnitException { get; private set; }
        public bool IsNUnitSuccessException { get; private set; }
        public bool IsNUnitInconclusiveException { get; private set; }
        public bool IsNUnitIgnoreException { get; private set; }
        public string NUnitExceptionMessage { get; private set; }

        private bool m_NeedToProcessLogs;
        private static List<LogScope> s_ActiveScopes = new List<LogScope>();

        internal static LogScope Current
        {
            get
            {
                if (s_ActiveScopes.Count == 0)
                    throw new InvalidOperationException("No log scope is available");
                return s_ActiveScopes[0];
            }
        }

        internal static bool HasCurrentLogScope()
        {
            return s_ActiveScopes.Count > 0;
        }

        public LogScope()
        {
            AllLogs = new List<LogEvent>();
            FailingLogs = new List<LogEvent>();
            ExpectedLogs = new Queue<LogMatch>();
            IgnoreFailingMessages = false;
            Activate();
        }

        private void Activate()
        {
            s_ActiveScopes.Insert(0, this);
            RegisterScope(this);
            Application.logMessageReceivedThreaded -= AddLog;
            Application.logMessageReceivedThreaded += AddLog;
        }

        private void Deactivate()
        {
            Application.logMessageReceivedThreaded -= AddLog;
            s_ActiveScopes.Remove(this);
            UnregisterScope(this);
        }

        private static void RegisterScope(LogScope logScope)
        {
            Application.logMessageReceivedThreaded += logScope.AddLog;
        }

        private static void UnregisterScope(LogScope logScope)
        {
            Application.logMessageReceivedThreaded -= logScope.AddLog;
        }

        public void AddLog(string message, string stacktrace, LogType type)
        {
            lock (_lock)
            {
                m_NeedToProcessLogs = true;
                var log = new LogEvent
                {
                    LogType = type,
                    Message = message,
                    StackTrace = stacktrace,
                };

                AllLogs.Add(log);

                if (IsNUnitResultStateException(stacktrace, type))
                {
                    if (message.StartsWith("SuccessException"))
                    {
                        IsNUnitException = true;
                        IsNUnitSuccessException = true;
                        if (message.StartsWith("SuccessException: "))
                        {
                            NUnitExceptionMessage = message.Substring("SuccessException: ".Length);
                            return;
                        }
                    }
                    else if (message.StartsWith("InconclusiveException"))
                    {
                        IsNUnitException = true;
                        IsNUnitInconclusiveException = true;
                        if (message.StartsWith("InconclusiveException: "))
                        {
                            NUnitExceptionMessage = message.Substring("InconclusiveException: ".Length);
                            return;
                        }
                    }
                    else if (message.StartsWith("IgnoreException"))
                    {
                        IsNUnitException = true;
                        IsNUnitIgnoreException = true;
                        if (message.StartsWith("IgnoreException: "))
                        {
                            NUnitExceptionMessage = message.Substring("IgnoreException: ".Length);
                            return;
                        }
                    }
                }

                if (IsFailingLog(type) && !IgnoreFailingMessages)
                {
                    FailingLogs.Add(log);
                }
            }
        }

        public bool IsAllLogsHandled()
        {
            lock (_lock)
            {
                return AllLogs.All(x => x.IsHandled);
            }
        }

        public LogEvent GetUnhandledLog()
        {
            lock (_lock)
            {
                return AllLogs.First(x => !x.IsHandled);
            }
        }

        private static bool IsNUnitResultStateException(string stacktrace, LogType logType)
        {
            if (logType != LogType.Exception)
                return false;

            return string.IsNullOrEmpty(stacktrace) || stacktrace.StartsWith("NUnit.Framework.Assert.");
        }

        private static bool IsFailingLog(LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    return true;
                default:
                    return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            m_Disposed = true;

            if (disposing)
            {
                Deactivate();
            }
        }

        internal bool AnyFailingLogs()
        {
            ProcessExpectedLogs();
            return FailingLogs.Any();
        }

        internal void ProcessExpectedLogs()
        {
            lock (_lock)
            {
                if (!m_NeedToProcessLogs || !ExpectedLogs.Any())
                    return;

                LogMatch expectedLog = null;
                foreach (var logEvent in AllLogs)
                {
                    if (!ExpectedLogs.Any())
                        break;
                    if (expectedLog == null && ExpectedLogs.Any())
                        expectedLog = ExpectedLogs.Peek();

                    if (expectedLog != null && expectedLog.Matches(logEvent))
                    {
                        ExpectedLogs.Dequeue();
                        logEvent.IsHandled = true;
                        if (FailingLogs.Any(expectedLog.Matches))
                        {
                            var failingLog = FailingLogs.First(expectedLog.Matches);
                            FailingLogs.Remove(failingLog);
                        }
                        expectedLog = null;
                    }
                }
                m_NeedToProcessLogs = false;
            }
        }
    }
}
