using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.Logger;

namespace NetworkLib.Common
{
    public class MessageFactory
    {
        protected readonly BinaryFormatter _binaryFormatter;
        protected readonly ILogNetwork _log;

        public MessageFactory(ILogNetwork log, BinaryFormatter binaryFormatter)
        {
            _binaryFormatter = binaryFormatter;
            _log = log;
        }

        public Message CreateMessage(string messageText)
        {
            return new Message(Serialize(messageText));
        }

        public Message CreateMessage(object messageObject)
        {
            return new Message(Serialize(messageObject));
        }

        public Message CreateMessage(byte[] messageData)
        {
            return new Message(messageData);
        }

        public T Deserialize<T>(byte[] data)
        {
            try
            {
                // Create a new byte array to hold a copy of the data
                byte[] dataCopy = new byte[data.Length];
                Array.Copy(data, dataCopy, data.Length);

                // Deserialize the copied data
                using (MemoryStream memoryStream = new MemoryStream(dataCopy))
                {
                    return (T)_binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch (Exception e)
            {
                _log.LogWarning($"Error deserializing data: {e}");
                //_log.LogWarning($"Corrupted data: {BitConverter.ToString(data)}");
                throw;
            }
        }

        public byte[] Serialize(object obj)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _binaryFormatter.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning($"Error serializing object: {ex.Message}");
                throw;
            }
        }
    }
}