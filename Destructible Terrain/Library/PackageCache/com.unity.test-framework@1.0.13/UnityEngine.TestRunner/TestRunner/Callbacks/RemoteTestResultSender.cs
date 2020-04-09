using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework.Interfaces;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.TestRunner.TestLaunchers;

namespace UnityEngine.TestTools.TestRunner.Callbacks
{
    [AddComponentMenu("")]
    internal class RemoteTestResultSender : MonoBehaviour, ITestRunnerListener
    {
        private class QueueData
        {
            public Guid id { get; set; }
            public byte[] data { get; set; }
        }

        private readonly Queue<QueueData> m_SendQueue = new Queue<QueueData>();
        private readonly object m_LockQueue = new object();
        private readonly IRemoteTestResultDataFactory m_TestResultDataFactory = new RemoteTestResultDataFactory();

        public void Start()
        {
            PlayerConnection.instance.Register(PlayerConnectionMessageIds.runFinishedMessageId, EditorProccessedTheResult);
            StartCoroutine(SendDataRoutine());
        }

        private void EditorProccessedTheResult(MessageEventArgs arg0)
        {
            if (arg0.data != null)
            {
                return;
            }

            //Some platforms don't quit, so we need to disconnect to make sure they will not connect to another editor instance automatically.
            PlayerConnection.instance.DisconnectAll();

            //XBOX has an error when quitting
            if (Application.platform == RuntimePlatform.XboxOne)
            {
                return;
            }
            Application.Quit();
        }

        private byte[] SerializeObject(object objectToSerialize)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(objectToSerialize));
        }

        public void RunStarted(ITest testsToRun)
        {
            var data = SerializeObject(m_TestResultDataFactory.CreateFromTest(testsToRun));
            lock (m_LockQueue)
            {
                m_SendQueue.Enqueue(new QueueData
                {
                    id = PlayerConnectionMessageIds.runStartedMessageId,
                    data = data
                });
            }
        }

        public void RunFinished(ITestResult testResults)
        {
            var data = SerializeObject(m_TestResultDataFactory.CreateFromTestResult(testResults));
            lock (m_LockQueue)
            {
                m_SendQueue.Enqueue(new QueueData { id = PlayerConnectionMessageIds.runFinishedMessageId, data = data, });
            }
        }

        public void TestStarted(ITest test)
        {
            var data = SerializeObject(m_TestResultDataFactory.CreateFromTest(test));
            lock (m_LockQueue)
            {
                m_SendQueue.Enqueue(new QueueData
                {
                    id = PlayerConnectionMessageIds.testStartedMessageId,
                    data = data
                });
            }
        }

        public void TestFinished(ITestResult result)
        {
            var testRunnerResultForApi = m_TestResultDataFactory.CreateFromTestResult(result);
            var resultData = SerializeObject(testRunnerResultForApi);
            lock (m_LockQueue)
            {
                m_SendQueue.Enqueue(new QueueData
                {
                    id = PlayerConnectionMessageIds.testFinishedMessageId,
                    data = resultData,
                });
            }
        }

        public IEnumerator SendDataRoutine()
        {
            while (!PlayerConnection.instance.isConnected)
            {
                yield return new WaitForSeconds(1);
            }

            while (true)
            {
                lock (m_LockQueue)
                {
                    if (PlayerConnection.instance.isConnected && m_SendQueue.Count > 0)
                    {
                        var queueData = m_SendQueue.Dequeue();
                        PlayerConnection.instance.Send(queueData.id, queueData.data);
                        yield return null;
                    }

                    //This is needed so we dont stall the player totally
                    if (!m_SendQueue.Any())
                    {
                        yield return new WaitForSeconds(0.02f);
                    }
                }
            }
        }
    }
}
