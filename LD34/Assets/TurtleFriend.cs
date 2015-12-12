using UnityEngine;
using System.Collections;

public class TurtleFriend : MonoBehaviour {

    public bool Out{
        get{
            return Captured || Saved;
        }
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
}
