using System;
using System.Collections.Generic;
using DTOs;
using UnityEngine;

namespace GameClient
{
    public class Network
    {
        public void Recieve(object obj)
        {
            if (obj is Batch)
            {
                Batch batch = (Batch)obj;
                Debug.Log(batch);
                ExecuteOnMainthread.RunOnMainThread.Enqueue(() => { GameController.Instance.BatchUpdate(batch); });
            }
            if (obj is Projectile)
            {
                Projectile projectile = (Projectile)obj;
                Debug.Log("Recieved projectile: " + projectile);
            }
            else if (obj is Projectile[])
            {
                Projectile[] projectiles = (Projectile[])obj;
                Debug.Log("Recieved projectiles: " + projectiles);
            }
            else if (obj is User)
            {
                User user = (User)obj;
                Debug.Log("Recieved user: " + user);
            }
        }

        public void RecieveUser(User user)
        {
            throw new NotImplementedException();
        }

        public void SendUser(User user)
        {
            throw new NotImplementedException();
        }

        public void SendProjectile(Projectile projectile)
        {
            throw new NotImplementedException();
        }

        public void RecieveProjectile(Projectile projectile)
        {
            throw new NotImplementedException();
        }
    }
}