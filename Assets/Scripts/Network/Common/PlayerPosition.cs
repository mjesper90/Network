using System;

namespace DTOs
{
    [Serializable]
    public class PlayerPosition
    {
        public string ID;
        public string Username;
        public Position Pos;
        public float Health;

        public PlayerPosition(string username, float x, float y, float z)
        {
            ID = Guid.NewGuid().ToString();
            Username = username;
            Pos = new Position(x, y, z);
            Health = 100f;
        }

        public PlayerPosition(string username, float x, float y, float z, float health) : this(username, x, y, z)
        {
            Health = health;
        }

        public PlayerPosition(string id, string username, float x, float y, float z, float health) : this(username, x, y, z, health)
        {
            ID = id;
        }

        public override string ToString()
        {
            return "Player: " + Username + ", " + Pos.X + ", " + Pos.Y + ", " + Pos.Z + ", " + Health;
        }
    }
}