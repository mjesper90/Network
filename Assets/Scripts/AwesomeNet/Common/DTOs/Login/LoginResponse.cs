using System;

namespace NetworkLib.Common.DTOs
{
    [Serializable]
    public class LoginResponse : Message
    {
        public bool Success { get; }
        public string Message { get; }

        public LoginResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}