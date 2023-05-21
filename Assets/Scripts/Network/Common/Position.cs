using System;

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

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}