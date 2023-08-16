using System;

namespace NetworkLib.Common.DTOs
{
    [Serializable]
    public class TestMessage : Message
    {
        public string Test { get; }

        public TestMessage(string test)
        {
            Test = test;
        }

        public TestMessage()
        {

        }

        public override string ToString()
        {
            return "TestMessage: " + Test;
        }
    }
}