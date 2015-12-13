using UnityEngine;
using System.Collections;

public class FunBunny : MonoBehaviour {

    public bool LoseCondition{
        get;
        set;
    }

    [ContextMenu("TestLose")]
    public void TestLose(){
        LoseCondition = true;
    }
}
