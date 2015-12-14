using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Life : MonoBehaviour {

    public List<Renderer> hearts = new List<Renderer>();
    public Animator animator;

    int lastNUm;

    public void UpdateHearts(int num){

        if(lastNUm != num){
            animator.Play("hearts",0,0);
        }

        for(int i = 1; i <= hearts.Count; i++){
            if(i <= num){
                hearts[i-1].enabled = true;
            }else {
                hearts[i-1].enabled = false;
            }
        }

        lastNUm = num;
    }
}
