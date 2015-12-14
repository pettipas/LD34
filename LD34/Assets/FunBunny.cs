using UnityEngine;
using System.Collections;

public class FunBunny : MonoBehaviour {

    public AudioSource takesHit;

    public bool LoseCondition{
        get;
        set;
    }
    BoxCollider collider;
    public void Start(){
        collider = GetComponent<BoxCollider>();
    }

    float duration =2.0f;
    float timer;

    public bool Invincible{
        get{
            return timer > 0;
        }
    }

    public void ResetInvincible(bool flash){
        timer = duration;
        if(flash) {
            takesHit.Play();
            GetComponent<MaterialFlasher>().FlashForTime(2.0f);
        }
    }

    public void Update(){

        if(timer > 0){
            collider.enabled = false;
        }else {
            collider.enabled = true;
        }

        if(timer > -1){
            timer-=Time.smoothDeltaTime;
        }
    }
}
