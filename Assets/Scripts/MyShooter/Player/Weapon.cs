using MyShooter.DTOs;
using MyShooter.NetworkSetup;
using UnityEngine;

namespace MyShooter
{
    public class Weapon : MonoBehaviour
    {
        public Transform Nozzle;
        public Player Owner;
        private GameObject _projectilePrefab;

        public void Start()
        {
            _projectilePrefab = Resources.Load(CONSTANTS.ProjectilePrefab) as GameObject;
        }

        public void PewPew(bool local = false)
        {
            if (ClientInit.Instance != null && ClientInit.Instance.Client != null)
            {
                BulletSpawn b = new BulletSpawn(Nozzle.position.x, Nozzle.position.y, Nozzle.position.z, Nozzle.forward.x, Nozzle.forward.y, Nozzle.forward.z);
                ClientInit.Instance.Client.Send(b);
            }
        }
    }
}