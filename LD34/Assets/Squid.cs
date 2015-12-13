using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squid : MovementScheme {
    
    float detectionDistance = 1.0f;
    public float movementRate = 3.0f;
    public float movementTimer;

    public bool chaseHero;

    public void Awake(){
    }

    public void Update(){
        movementTimer-=Time.deltaTime;

        if(movementTimer<0 && !actionRunning){
            movementTimer = movementRate;
            actionRunning = true;
            StartCoroutine(StepAndTurn(GetNextPosition()));
        }


        if(!ActionRunning){// then we can check to see if we are near a turtle

            Collider[] detected = Physics.OverlapBox(transform.position, new Vector3(0.6f,0.6f,0.6f));
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

    bool DetectedNeighbor(RaycastHit[] rightHits){
        int count = 0;
        for(int i = 0; i < rightHits.Length;i++){
            Squid squid =  rightHits[i].transform.GetComponent<Squid>();
            if(squid != null && squid != this){
                count++;
            }
        }
        return count != 0;
    }

    public Vector3 GetNextPosition(){

        List<Vector3> nextPositions = new List<Vector3>();
        List<RaycastHit> allHits = new List<RaycastHit>();
        RaycastHit[] rightHits = Physics.RaycastAll(transform.position,transform.right, detectionDistance);
        RaycastHit[] leftHits = Physics.RaycastAll(transform.position,-transform.right, detectionDistance);
        RaycastHit[] forwardHits = Physics.RaycastAll(transform.position,transform.forward,detectionDistance);
        RaycastHit[] behindHits = Physics.RaycastAll(transform.position,-transform.forward,detectionDistance);

        if(!DetectedNeighbor(rightHits)){
            nextPositions.Add(transform.position + transform.right *1.0f);
        }

        if(!DetectedNeighbor(leftHits)){
            nextPositions.Add(transform.position + -transform.right *1.0f);
        }

        if(!DetectedNeighbor(forwardHits)){
            nextPositions.Add(transform.position + transform.forward *1.0f);
        }

        if(!DetectedNeighbor(behindHits)){
            nextPositions.Add(transform.position + -transform.forward *1.0f);
        }

        Vector3 p = GameLoop.Instance.ChooseAIPosition(nextPositions, transform.position, chaseHero);
        return p;
    }
}
