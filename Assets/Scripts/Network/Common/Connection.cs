using System;

namespace NetworkLib
{
    [Serializable]
    public class Connection
    {
        public string Username;
        public string Password;

        public Connection(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}