using System;

namespace MyGame.DTOs
{
    [Serializable]
    public class PlayerRotation
    {
        public string Username;
        public float Y;

        public PlayerRotation(string username, float y)
        {
            this.Username = username;
            this.Y = (float)Math.Round(y, 3);
        }
    }
}