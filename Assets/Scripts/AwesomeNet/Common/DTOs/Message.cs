using System;

namespace NetworkLib.Common.DTOs
{
    [Serializable]
    public class Message
    {
        public string Id;
        public byte[] Data;
        public string Callback;

        public Message(byte[] data, string callback)
        {
            Id = Guid.NewGuid().ToString();
            Data = data;
            Callback = callback;
        }

        public Message(byte[] data)
        {
            Id = Guid.NewGuid().ToString();
            Data = data;
            Callback = "";
        }

        public Message()
        {
            Id = Guid.NewGuid().ToString();
            Data = new byte[0];
            Callback = "";
        }

        public override string ToString()
        {
            return "Message: of type with " + Data.Length + " bytes of data and callback id " + Callback;
        }
    }
}