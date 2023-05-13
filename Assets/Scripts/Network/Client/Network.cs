using System.Collections.Concurrent;
using DTOs;

namespace GameClient
{
    public class Network
    {
        public ConcurrentQueue<Batch> ActionQueue = new ConcurrentQueue<Batch>();

        public void Receive(object obj)
        {
            if (obj is Batch batch)
            {
                ActionQueue.Enqueue(batch);
            }
            if (obj is Projectile)
            {
                Projectile projectile = (Projectile)obj;
            }
            else if (obj is Projectile[])
            {
                Projectile[] projectiles = (Projectile[])obj;
            }
            else if (obj is User)
            {
                User user = (User)obj;
            }
        }
    }
}