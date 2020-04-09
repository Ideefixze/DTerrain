using System;

namespace UnityEngine.TestRunner.TestLaunchers
{
    internal static class PlayerConnectionMessageIds
    {
        public static Guid runStartedMessageId { get { return new Guid("6a7f53dd-4672-461d-a7b5-9467e9393fd3"); } }
        public static Guid runFinishedMessageId { get { return new Guid("ffb622fc-34ad-4901-8d7b-47fb04b0bdd4"); } }
        public static Guid testStartedMessageId { get { return new Guid("b54d241e-d88d-4dba-8c8f-ee415d11c030"); } }
        public static Guid testFinishedMessageId { get { return new Guid("72f7b7f4-6829-4cd1-afde-78872b9d5adc"); } }
    }
}
