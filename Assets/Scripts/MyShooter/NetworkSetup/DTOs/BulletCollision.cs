using System;
using NetworkLib.Common.DTOs;

namespace MyShooter.DTOs
{
    [Serializable]
    public class BulletCollision : Message
    {
        public string BulletId;
        public string PlayerId;

        public BulletCollision(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return "";
        }
    }
}