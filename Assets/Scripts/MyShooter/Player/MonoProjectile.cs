using System;
using System.Collections;
using MyShooter.DTOs;
using UnityEngine;

public class MonoProjectile : MonoBehaviour
{
    public event Action<Collision, MonoProjectile> OnCollision;
    public BulletSpawn BulletSpawn { get; set; }

    void OnCollisionEnter(Collision collision)
    {
        if (OnCollision != null)
        {
            OnCollision(collision, this);
        }
        else
        {
            Debug.Log("No collision event");
        }
    }
}
