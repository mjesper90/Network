using System;

namespace MyGame.DTOs
{
    [Serializable]
    public class User
    {
        public string ID;
        public string Username;

        public User(string username)
        {
            ID = Guid.NewGuid().ToString();
            Username = username;
        }

        public User(string id, string username) : this(username)
        {
            ID = id;
        }

        public override string ToString()
        {
            return "User: " + Username;
        }
    }
}