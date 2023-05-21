using System;
using System.Collections.Generic;

namespace DTOs
{
    [Serializable]
    public class Message
    {
        public string Id;
        public MessageType MsgType;
        public byte[] Data;
        public string Callback;

        public Message(MessageType msgType, byte[] data, string callback)
        {
            Id = Guid.NewGuid().ToString();
            MsgType = msgType;
            Data = data;
            Callback = callback;
        }

        public override string ToString()
        {
            return "Message: of type " + MsgType + " with " + Data.Length + " bytes of data and callback id " + Callback;
        }
    }
}