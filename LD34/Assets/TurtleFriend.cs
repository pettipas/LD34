using UnityEngine;
using System.Collections;

public class TurtleFriend : MonoBehaviour {

    public Transform body;
    public AudioSource pop;
    public AudioSource save;
    public void Awake(){
        body.eulerAngles+=new Vector3(0,-90*Random.Range(1,7),0);
    }

    IEnumerator Start(){
        pop.Play();
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(3.0f);
        GetComponent<BoxCollider>().enabled = true;
        yield break;
    }

    public bool Out{
        get{
            return (Captured || Saved);
        }
    }

    public bool IsFlying{
        get{
            return running;
        }
    }

    public void FlyToPosition(Transform flyTo, ParticleSystem vortex){
        
        GetComponentInChildren<Animator>().Play("fly",0,0);
        if(!running){
           
            running = true;
            StartCoroutine(FlyToPosition(flyTo.transform.position,vortex));
        }
    }
    bool running = false;

    IEnumerator FlyToPosition(Vector3 validNextPosition, ParticleSystem system){
        yield return new WaitForSeconds(1.0f);
        float timeFrag = 1/2f;
        float t = 0;
        Vector3 startPosition = transform.position;
        Vector3 lookDir = (validNextPosition - transform.position).normalized;
        body.transform.forward = lookDir;
        while(t < 1){
            system.Emit(30);
            transform.position = Vector3.Lerp(startPosition, validNextPosition, t);
            t+=timeFrag*Time.smoothDeltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        running = false;
    }

    [ContextMenu("TestCapture")]
    public void TestCapture(){
        Captured = true;
    }

    [ContextMenu("TestSaved")]
    public void TestSaved(){
        Saved = true;
    }

    public bool Captured{
        get;
        set;
    }

    public bool Saved{
        get;
        set;
    }

    void OnDrawGizmos(){
        
        if(Captured){
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position,Vector3.one);
        }

        if(Saved){
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position,Vector3.one);
        }
    }

    public void SaveTurtle(){
        save.Play();
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInChildren<Animator>().Play("flip",0,0);
    }

    public void CaptureTurtle(){
        GetComponentInChildren<Animator>().Play("death_flip",0,0);
    }
}
