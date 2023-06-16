using System;

namespace NetworkLib.Common.DTOs
{
    [Serializable]
    public class Authentication : Message
    {
        public string Username { get; }
        public string Password { get; }

        public Authentication(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}