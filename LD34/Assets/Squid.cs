using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squid : MovementScheme {

    public float movementRate = 3.0f;
    public float movementTimer;

    public void Update(){
        movementTimer-=Time.deltaTime;

        if(movementTimer<0 && !actionRunning){
            movementTimer = movementRate;
            actionRunning = true;
            StartCoroutine(StepAndTurn(Vector3.zero));
        }


        if(!ActionRunning){// then we can check to see if we are near a turtle

            Collider[] detected = Physics.OverlapBox(transform.position, new Vector3(0.4f,0.4f,0.4f));
            for(int i = 0; i < detected.Length;i++){
                TurtleFriend tf = detected[i].transform.GetComponent<TurtleFriend>();

                if(tf != null 
                    && !tf.Captured 
                    && !tf.Saved
                    ){
                    
                    GameLoop.Instance.OnCapturedTurtle(tf);
                }
            }
        }
    }

    public Vector3 GetNextPosition(){

        List<Vector3> nextPositions = new List<Vector3>();

        RaycastHit hitRight;
        if(Physics.Raycast(transform.position,transform.right, out hitRight, 1.0f)){
            Squid s = hitRight.transform.GetComponent<Squid>();
            if(s == null){
                nextPositions.Add(transform.position + transform.right *1.0f);
            }
        }

        RaycastHit hitLeft;
        if(Physics.Raycast(transform.position,-transform.right,out hitLeft, 1.0f)){
            Squid s = hitLeft.transform.GetComponent<Squid>();
            if(s == null){
                nextPositions.Add(transform.position + -transform.right *1.0f);
            }
        }

        RaycastHit hitBack;
        if(Physics.Raycast(transform.position,transform.forward, out hitBack, 1.0f)){
            Squid s = hitBack.transform.GetComponent<Squid>();
            if(s == null){
                nextPositions.Add(transform.position + transform.forward *1.0f);
            }
        }

        RaycastHit hitForward;
        if(Physics.Raycast(transform.position,-transform.forward, out hitForward, 1.0f)){
            Squid s = hitForward.transform.GetComponent<Squid>();
            if(s == null){
                nextPositions.Add(transform.position + -transform.forward *1.0f);
            }
        }

        Vector3 p = GameLoop.Instance.ChooseAIPosition(nextPositions, transform.position);
        return p;
    }
}
