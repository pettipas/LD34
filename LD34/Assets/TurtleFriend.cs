using UnityEngine;
using System.Collections;

public class TurtleFriend : MonoBehaviour {

    public Transform body;

    public void Awake(){
        body.eulerAngles+=new Vector3(0,-90*Random.Range(1,7),0);
    }

    public bool Out{
        get{
            return (Captured || Saved);
        }
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
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInChildren<Animator>().Play("flip",0,0);
    }

    public void CaptureTurtle(){
        GetComponentInChildren<Animator>().Play("death_flip",0,0);
    }
}
