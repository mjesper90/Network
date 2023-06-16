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

        public MonoProjectile PewPew(bool local = false)
        {
            GameObject projectile = Instantiate(_projectilePrefab, Nozzle.position, Nozzle.rotation);
            MonoProjectile p = projectile.GetComponent<MonoProjectile>();

            p.Launch(local);
            return p;
        }
    }
}