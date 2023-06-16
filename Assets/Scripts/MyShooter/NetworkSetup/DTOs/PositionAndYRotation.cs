using System;
using NetworkLib.Common.DTOs;

namespace MyShooter.DTOs
{
    [Serializable]
    public class PositionAndYRotation : Message
    {
        public string Username { get; }
        public float X, Y, Z, YRotation;

        public PositionAndYRotation(string username, float x, float y, float z, float yr, int precision = 3)
        {
            Username = username;
            X = (float)Math.Round(x, precision);
            Y = (float)Math.Round(y, precision);
            Z = (float)Math.Round(z, precision);
            YRotation = (float)Math.Round(yr, precision);
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + "), Y-Rotation: " + YRotation;
        }
    }
}