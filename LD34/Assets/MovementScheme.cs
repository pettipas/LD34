using UnityEngine;
using System.Collections;

public abstract class MovementScheme : MonoBehaviour {

    public AudioSource hop;
    public AudioSource step;
    public Animator animator;
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

        if(animator == null){
            animator = GetComponentInChildren<Animator>();
        }
        step.Play();
        animator.Play("stride",0,0);
        float timeFrag = 1/moveSpeed;
        float t = 0;
        Vector3 destination = transform.position +  body.transform.forward * 1.0f;

        if(destination.x < -1 ||
            destination.z < -1 ||
            destination.x>11||
            destination.z>11){
            Debug.Log("blocked");
            actionRunning = false;
            yield break;
        }

        Vector3 startPosition = transform.position;
        while(t < 1){
            transform.position = Vector3.Lerp(startPosition, destination,t);
            t+=timeFrag*Time.smoothDeltaTime;
            yield return null;
        }
        animator.Play("breath",0,0);
        transform.position = destination;
        actionRunning = false;
        yield break;
    }

    public IEnumerator Turn() {
        hop.Play();
        if(animator == null){
            animator = GetComponentInChildren<Animator>();
        }

        animator.Play("hop",0,0);
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
