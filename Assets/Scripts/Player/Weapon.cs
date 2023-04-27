using System.Collections;
using DTOs;
using UnityEngine;

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

        Position start = new Position(Nozzle.position.x, Nozzle.position.y, Nozzle.position.z);
        Position end = new Position(Nozzle.forward.x, Nozzle.forward.y, Nozzle.forward.z);
        Projectile proj = new Projectile(Owner.UserInfo, start, end, 1f);
        p.Launch(proj, local);
        return p;
    }
}