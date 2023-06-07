using System;

namespace MyGame.DTOs
{
    [Serializable]
    public class Bullet
    {
        public Position Position;
        public Position Velocity;

        public Bullet(float x, float y, float z, float vx, float vy, float vz)
        {
            Position = new Position(x, y, z);
            Velocity = new Position(vx, vy, vz);
        }

        public override string ToString()
        {
            return "Bullet: " + Position.X + ", " + Position.Y + ", " + Position.Z + ", " + Velocity.X + ", " + Velocity.Y + ", " + Velocity.Z;
        }
    }
}