using System;

namespace MyGame.DTOs
{
    [Serializable]
    public class PlayerPosition
    {
        public string Username;
        public Position Pos;

        public PlayerPosition(string username, float x, float y, float z)
        {
            Username = username;
            Pos = new Position(x, y, z);
        }

        public override string ToString()
        {
            return "PlayerPosition: " + Username + ", " + Pos.X + ", " + Pos.Y + ", " + Pos.Z;
        }
    }
}