using System.Text.RegularExpressions;
using UnityEngine.TestTools.Logging;
using UnityEngine.TestTools.TestRunner;

namespace UnityEngine.TestTools
{
    public static class LogAssert
    {
        public static void Expect(LogType type, string message)
        {
            LogScope.Current.ExpectedLogs.Enqueue(new LogMatch() { LogType = type, Message = message });
        }

        public static void Expect(LogType type, Regex message)
        {
            LogScope.Current.ExpectedLogs.Enqueue(new LogMatch() { LogType = type, MessageRegex = message });
        }

        public static void NoUnexpectedReceived()
        {
            LogScope.Current.ProcessExpectedLogs();
            var isAllLogsHandled = LogScope.Current.IsAllLogsHandled();
            if (isAllLogsHandled)
            {
                return;
            }
            var unhandledLog = LogScope.Current.GetUnhandledLog();
            throw new UnhandledLogMessageException(unhandledLog);
        }

        public static bool ignoreFailingMessages
        {
            get
            {
                return LogScope.Current.IgnoreFailingMessages;
            }
            set
            {
                LogScope.Current.IgnoreFailingMessages = value;
            }
        }
    }
}
