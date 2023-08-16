using System;
using NetworkLib.Common.DTOs;

namespace MyShooter.DTOs
{
    [Serializable]
    public class BulletSpawn : Message
    {
        public Vec3 Position;
        public Vec3 Direction;

        public BulletSpawn(float x, float y, float z, float vx, float vy, float vz)
        {
            Position = new Vec3(x, y, z);
            Direction = new Vec3(vx, vy, vz);
        }

        public override string ToString()
        {
            return "Bullet: at " + Position + " with direction " + Direction;
        }
    }
}