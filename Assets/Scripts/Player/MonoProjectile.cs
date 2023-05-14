using System.Collections;
using DTOs;
using UnityEngine;

//#TODO: Figure out what to do with this
public class MonoProjectile : MonoBehaviour
{
    public Projectile Projectile;

    public void Launch(Projectile p, bool local = false)
    {
        Projectile = p;
        if (local)
            GetComponent<Rigidbody>().AddForce(new Vector3(p.End.X, p.End.Y, p.End.Z) * CONSTANTS.ProjectileSpeed);
        Destroy(gameObject, 5f);
    }

    public void LerpMovement(Vector3 vector3)
    {
        StartCoroutine(LerpMovementCoroutine(vector3, CONSTANTS.ServerSpeed));
    }

    //Lerp movement coroutine
    private IEnumerator LerpMovementCoroutine(Vector3 vector3, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, vector3, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = vector3;
    }
}
