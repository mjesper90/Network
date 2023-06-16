using System;
using NetworkLib.Common.DTOs;

namespace MyShooter.DTOs
{
    [Serializable]
    public class Bullet : Message
    {
        public Vec3 Position;
        public Vec3 Direction;

        public Bullet(float x, float y, float z, float vx, float vy, float vz)
        {
            Position = new Vec3(x, y, z);
            Direction = new Vec3(vx, vy, vz);
        }

        public override string ToString()
        {
            return "Bullet: " + Position.X + ", " + Position.Y + ", " + Position.Z + ", " + Direction.X + ", " + Direction.Y + ", " + Direction.Z;
        }
    }
}