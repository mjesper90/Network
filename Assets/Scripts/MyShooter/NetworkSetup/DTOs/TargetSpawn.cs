using System;
using NetworkLib.Common.DTOs;

namespace MyShooter.DTOs
{
    [Serializable]
    public class TargetSpawn : Message
    {
        public Vec3 Position;

        public TargetSpawn(float x, float y, float z)
        {
            Position = new Vec3(x, y, z);
        }

        public override string ToString()
        {
            return "Target at " + Position;
        }
    }
}