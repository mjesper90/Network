using System.Collections;
using UnityEngine;

//#TODO: Figure out what to do with this
public class MonoProjectile : MonoBehaviour
{
    public void Launch(bool local = false)
    {
        if (local)
            
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
