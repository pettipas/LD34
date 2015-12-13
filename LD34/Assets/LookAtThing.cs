using UnityEngine;
using System.Collections;

public class LookAtThing : MonoBehaviour {

    public Transform thing;

    public void Start(){
      
    }
    Ctrl ctrl;
    public void Update(){

        if(thing != null){
            ctrl = thing.GetComponent<Ctrl>();
        }

        if(thing != null 
            && thing.transform.position.x>3.5 
            && thing.transform.position.x<7.5){
            if(thing!= null){
                transform.LookAt(ctrl.animation);
            }
        }
    }
}
