using UnityEngine;
using System.Collections;

public abstract class MovementScheme : MonoBehaviour {

    public Transform body;
    public bool actionRunning;
    public float moveSpeed = 0.5f;

    public bool ActionRunning{
        get{
            return actionRunning;
        }
    }

    Vector3 nextDest;
    public IEnumerator StepAndTurn(Vector3 validNextPosition){
        float timeFrag = 1/moveSpeed;
        float t = 0;
        nextDest = validNextPosition;
        Vector3 startPosition = transform.position;
        Vector3 lookDir = (validNextPosition - transform.position).normalized;
        body.transform.forward = lookDir;
        while(t < 1){
            transform.position = Vector3.Lerp(startPosition, validNextPosition, t);
            t+=timeFrag*Time.smoothDeltaTime;
            yield return null;
        }
        transform.position = validNextPosition;
        actionRunning = false;
        yield break;
    }

    public IEnumerator Step(){
        float timeFrag = 1/0.5f;
        float t = 0;
        Vector3 destination = transform.position +  body.transform.forward * 1.0f;
        Vector3 startPosition = transform.position;
        while(t < 1){
            transform.position = Vector3.Lerp(startPosition, destination,t);
            t+=timeFrag*Time.smoothDeltaTime;
            yield return null;
        }
        transform.position = destination;
        actionRunning = false;
        yield break;
    }

    public IEnumerator Turn(){
        body.transform.eulerAngles += new Vector3(0,90,0);
        yield return new WaitForSeconds(0.2f);
        actionRunning = false;
        yield break;
    }

    public void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,nextDest);
        Gizmos.DrawSphere(nextDest,0.5f);
    }
}
