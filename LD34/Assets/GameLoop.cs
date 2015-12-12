using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {

    public Transform startPosition;
    public FunBunny bunny;

	void Start () {
        bunny.transform.position = startPosition.position;
	}
	
    IEnumerator Game(){


        yield break;
    }
}
