using System;

namespace NetworkLib.Common.DTOs
{
    [Serializable]
    public class Authentication
    {
        public string Username;
        public string Password;

        public Authentication(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}