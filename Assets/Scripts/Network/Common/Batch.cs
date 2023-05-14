using System;
using System.Collections.Generic;

namespace DTOs
{
    [Serializable]
    public class Batch
    {
        public Projectile[] Projectiles;
        public User[] Users;

        public Batch(Projectile[] projectiles, User[] users)
        {
            Projectiles = projectiles;
            Users = users;
        }

        public void Append(Batch batch)
        {
            List<Projectile> projectiles = new List<Projectile>(Projectiles);
            projectiles.AddRange(batch.Projectiles);
            Projectiles = projectiles.ToArray();

            List<User> users = new List<User>(Users);
            users.AddRange(batch.Users);
            Users = users.ToArray();
        }

        public override string ToString()
        {
            string projectiles = "";
            foreach (Projectile projectile in Projectiles)
            {
                projectiles += projectile + "\n";
            }

            string users = "";
            foreach (User user in Users)
            {
                users += user + "\n";
            }

            return "Batch: \n" + projectiles + users;
        }
    }
}