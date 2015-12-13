using UnityEngine;
using System.Collections;

public class LookAtThing : MonoBehaviour {

    public Transform thing;

    public void Update(){

        if(thing.transform.position.x>4 && thing.transform.position.x<6){
            if(thing!= null){
                transform.LookAt(thing);
            }
        }
    }
}
