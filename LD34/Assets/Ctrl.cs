using UnityEngine;
using System.Collections;

public class Ctrl : MonoBehaviour {

    public Transform body;

    bool actionRunning;

	void Update () {
        if(Input.GetKeyUp(KeyCode.D) && !actionRunning){
            actionRunning = true;
            StartCoroutine(Step());
        }

        if(Input.GetKeyUp(KeyCode.A) && !actionRunning){
            actionRunning = true;
            StartCoroutine(Turn());
        }
	}

    IEnumerator Step(){
        float timeFrag = 1/0.5f;
        float t = 0;
        Vector3 destination = transform.position +  body.transform.forward * 1.0f;
        Vector3 startPosition = transform.position;
        while(t < 1){
            transform.position = Vector3.Lerp(startPosition, destination,t);
            t+=timeFrag*Time.smoothDeltaTime;
            yield return null;
        }
        actionRunning = false;
        yield break;
    }

    IEnumerator Turn(){
        body.transform.eulerAngles += new Vector3(0,90,0);
        yield return new WaitForSeconds(0.2f);
        actionRunning = false;
        yield break;
    }
}
