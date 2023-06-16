using System;

namespace NetworkLib.Common.DTOs
{
    [Serializable]
    public class Vec3 : Message
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return "Vec3: " + X + ", " + Y + ", " + Z;
        }
    }
}