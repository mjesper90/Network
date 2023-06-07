using System;

namespace NetworkLib.Common.DTOs
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

        public Message(MessageType msgType, byte[] data)
        {
            Id = Guid.NewGuid().ToString();
            MsgType = msgType;
            Data = data;
            Callback = "";
        }
        public Message(MessageType msgType)
        {
            Id = Guid.NewGuid().ToString();
            MsgType = msgType;
            Data = new byte[0];
            Callback = "";
        }

        public override string ToString()
        {
            return "Message: of type " + MsgType + " with " + Data.Length + " bytes of data and callback id " + Callback;
        }
    }
}