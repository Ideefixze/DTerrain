using System;
using UnityEditor.Networking.PlayerConnection;
using UnityEditor.TestTools.TestRunner;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.TestTools.TestRunner.UnityTestProtocol;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.TestRunner.TestLaunchers;

namespace UnityEditor.TestRunner.TestLaunchers
{
    [Serializable]
    internal class RemoteTestRunController : ScriptableSingleton<RemoteTestRunController>
    {
        [SerializeField]
        private RemoteTestResultReciever m_RemoteTestResultReciever;

        [SerializeField]
        private PlatformSpecificSetup m_PlatformSpecificSetup;

        [SerializeField]
        private bool m_RegisteredConnectionCallbacks;

        public void Init(BuildTarget buildTarget)
        {
            m_PlatformSpecificSetup = new PlatformSpecificSetup(buildTarget);
            m_PlatformSpecificSetup.Setup();
            m_RemoteTestResultReciever = new RemoteTestResultReciever();
            EditorConnection.instance.Initialize();
            if (!m_RegisteredConnectionCallbacks)
            {
                EditorConnection.instance.Initialize();
                DelegateEditorConnectionEvents();
            }
        }

        private void DelegateEditorConnectionEvents()
        {
            m_RegisteredConnectionCallbacks = true;
            //This is needed because RemoteTestResultReciever is not a ScriptableObject
            EditorConnection.instance.Register(PlayerConnectionMessageIds.runStartedMessageId, RunStarted);
            EditorConnection.instance.Register(PlayerConnectionMessageIds.runFinishedMessageId, RunFinished);
            EditorConnection.instance.Register(PlayerConnectionMessageIds.testStartedMessageId, TestStarted);
            EditorConnection.instance.Register(PlayerConnectionMessageIds.testFinishedMessageId, TestFinished);
        }

        private void RunStarted(MessageEventArgs messageEventArgs)
        {
            m_RemoteTestResultReciever.RunStarted(messageEventArgs);
            CallbacksDelegator.instance.RunStartedRemotely(messageEventArgs.data);
        }

        private void RunFinished(MessageEventArgs messageEventArgs)
        {
            m_RemoteTestResultReciever.RunFinished(messageEventArgs);
            m_PlatformSpecificSetup.CleanUp();

            CallbacksDelegator.instance.RunFinishedRemotely(messageEventArgs.data);
        }

        private void TestStarted(MessageEventArgs messageEventArgs)
        {
            CallbacksDelegator.instance.TestStartedRemotely(messageEventArgs.data);
        }

        private void TestFinished(MessageEventArgs messageEventArgs)
        {
            CallbacksDelegator.instance.TestFinishedRemotely(messageEventArgs.data);
        }

        public void PostBuildAction()
        {
            m_PlatformSpecificSetup.PostBuildAction();
        }

        public void PostSuccessfulBuildAction()
        {
            m_PlatformSpecificSetup.PostSuccessfulBuildAction();
        }
    }
}
