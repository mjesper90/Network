using System;

namespace DTOs
{
    [Serializable]
    public class Projectile
    {
        public Position Start;
        public Position End;
        public User Owner;
        public string ID;
        public float Damage;

        public Projectile(User owner, Position start, Position end, float damage)
        {
            ID = Guid.NewGuid().ToString();
            Start = start;
            End = end;
            Owner = owner;
            Damage = damage;
        }

        public override string ToString()
        {
            return "Projectile: " + ID + " from " + Start + " to " + End + " owned by " + Owner.Username + " with damage " + Damage;
        }
    }
}