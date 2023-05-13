using System;
using System.Collections.Generic;
using DTOs;

namespace GameServer
{
    public class Network
    {
        private ServerClient ServerClientRef;
        public User UserRef;
        public List<Projectile> Projectiles = new List<Projectile>();

        public Network(ServerClient serverClient)
        {
            this.ServerClientRef = serverClient;
            UserRef = new User("Missing", 0, 0, 0);
        }

        public Batch GetBatch()
        {
            Batch b;
            if (UserRef.Username == "Missing")
            {
                b = new Batch(new Projectile[0], new User[0]);
            }
            else
            {
                b = new Batch(Projectiles.ToArray(), new User[] { UserRef });
            }
            Projectiles.Clear();
            return b;
        }

        public void Recieve(object obj)
        {
            if (obj is Projectile)
            {
                Projectile projectile = (Projectile)obj;
                Projectiles.Add(projectile);
            }
            else if (obj is User)
            {
                UserRef = (User)obj;
            }
            else if (obj is Position)
            {
                Position pos = (Position)obj;
                UserRef.Pos = pos;
            }
        }

        public void SendBatch(Batch batch)
        {
            ServerClientRef.Send(ServerClientRef.Serialize(batch));
        }

        public void SendUser(User user)
        {
            ServerClientRef.Send(ServerClientRef.Serialize(user));
        }

        public void SendProjectile(Projectile projectile)
        {
            ServerClientRef.Send(ServerClientRef.Serialize(projectile));
        }
    }
}