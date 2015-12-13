using UnityEngine;
using System.Collections;

public class Ctrl : MovementScheme {

    public Transform animation;

    public bool Blocked{
        get;
        set;
    }

	void Update () {
        if(Blocked){
            return;
        }
        if(Input.GetKeyUp(KeyCode.D) && !actionRunning){
            actionRunning = true;
            StartCoroutine(Step());
        }

        if(Input.GetKeyUp(KeyCode.A) && !actionRunning){
            actionRunning = true;
            StartCoroutine(Turn());
        }
	}


}
