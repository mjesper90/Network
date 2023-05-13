using System;
using UnityEngine;
namespace DTOs
{
    [Serializable]
    public class Position
    {
        public float X, Y, Z;

        public Position(float x, float y, float z, int precision = 3)
        {
            X = (float)Math.Round(x, precision);
            Y = (float)Math.Round(y, precision);
            Z = (float)Math.Round(z, precision);
        }

        public Position(Vector3 vector, int precision = 3)
        {
            X = (float)Math.Round(vector.x, precision);
            Y = (float)Math.Round(vector.y, precision);
            Z = (float)Math.Round(vector.z, precision);
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}