using System;
using UnityEditor.TestRunner.TestLaunchers;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace UnityEditor.TestTools.TestRunner.CommandLineTest
{
    internal class TimeoutCallbacks : ScriptableObject, ICallbacks
    {
        internal const int k_DefaultTimeout = 600;

        private Func<Action, double, IDelayedCallback> m_DelayedCallbackFactory;
        private Action<string, object[]> m_LogErrorFormat;
        private Action<int> m_ExitApplication;

        private double m_CurrentTimeout;
        private IDelayedCallback m_TimeoutCallback;

        public void Init(Func<Action, double, IDelayedCallback> delayedCallbackFactory, Action<string, object[]> logErrorFormat, Action<int> exitApplication)
        {
            m_DelayedCallbackFactory = delayedCallbackFactory;
            m_LogErrorFormat = logErrorFormat;
            m_ExitApplication = exitApplication;
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            if (m_TimeoutCallback != null)
            {
                m_TimeoutCallback.Clear();
            }
        }

        public void RunStarted(ITestAdaptor testsToRun)
        {
            ResetToTimeout(k_DefaultTimeout);
        }

        public void TestFinished(ITestResultAdaptor result)
        {
            ResetToTimeout(k_DefaultTimeout);
        }

        public void TestStarted(ITestAdaptor test)
        {
            ResetToTimeout(k_DefaultTimeout + test.TestCaseTimeout / 1000);
        }

        private void ResetToTimeout(double timeoutValue)
        {
            if (m_TimeoutCallback != null && Math.Abs(m_CurrentTimeout - timeoutValue) < 0.1f)
            {
                m_TimeoutCallback.Reset();
            }
            else
            {
                if (m_TimeoutCallback != null)
                {
                    m_TimeoutCallback.Clear();
                }

                m_TimeoutCallback = m_DelayedCallbackFactory(TimeoutReached, timeoutValue);
                m_CurrentTimeout = timeoutValue;
            }
        }

        private void TimeoutReached()
        {
            RemotePlayerLogController.instance.StopLogWriters();
            m_LogErrorFormat("Test execution timed out.", new object[0]);
            m_ExitApplication((int)Executer.ReturnCodes.RunError);
        }
    }
}
