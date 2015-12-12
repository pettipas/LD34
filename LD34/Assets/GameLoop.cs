using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameLoop : MonoBehaviour {

    public Transform startPosition;
    public FunBunny bunny;
    public float worldWidth = 10.0f;
    public float worldHeight = 10.0f;
    public FunBunny funBunnyPrefab;
    public TurtleFriend turtlePrefab;
    public Squid squidPrefab;
    public int level;
    public int turtlesSaved;
    public int turtlesCaptured;
    public static GameLoop Instance;
  
    public List<Squid> squids = new List<Squid>();
    public List<TurtleFriend> turtleFriends = new List<TurtleFriend>();

    void Awake(){
        if(Instance != null){
            Instance = null;
        }
        Instance = this;
    }

    void Start () {
        bunny = funBunnyPrefab.Duplicate(startPosition.position);
        bunny.LoseCondition = false;
        bunny.GetComponent<Ctrl>().Blocked = false;
        StartCoroutine(Game());
	}

    public Vector3 ChooseAIPosition(List<Vector3> possiblePositions, Vector3 defaultPos){
        if(possiblePositions.Count == 0){
            return defaultPos;
        }

        if(possiblePositions.Count == 1){
            return possiblePositions[0];
        }

        List<TurtleFriend> uncaptured = turtleFriends.FindAll(x=>x.Captured == false);
        TurtleFriend closestTurtle = uncaptured.OrderBy(x=>Vector3.Distance(x.transform.position,transform.position)).FirstOrDefault();

        if(closestTurtle == null){
            return defaultPos;
        }

        Vector3 closestToBunny;
        Vector3 closestToTurtle;

        Vector3 choice;

        closestToBunny = possiblePositions[0];
        float distanceToBunny = Vector3.Distance(closestToBunny, bunny.transform.position);

        for(int i = 0; i < possiblePositions.Count; i++){
            Vector3 v = possiblePositions[i];
            float testDistance = Vector3.Distance(v,bunny.transform.position);
            if(testDistance < distanceToBunny){
                distanceToBunny = testDistance;
                closestToBunny = v;
            }
        }

        closestToTurtle = possiblePositions[0];
        float distanceToTurtle = Vector3.Distance(closestToTurtle, closestTurtle.transform.position);
        for(int i = 0; i < possiblePositions.Count; i++){
            Vector3 v = possiblePositions[i];
            float testDistance = Vector3.Distance(v,closestTurtle.transform.position);
            if(testDistance < distanceToBunny){
                distanceToBunny = testDistance;
                closestToBunny = v;
            }
        }

        if(Random.value < 0.5f){
            return closestToBunny;
        }

        return closestToTurtle;
    }

    void OnSavedTurtle(TurtleFriend saved){
        saved.Saved = true;
        Debug.Log("Saved Turtle");
    }

    public void OnCapturedTurtle(TurtleFriend captured){
        captured.Captured = true;
        turtlesCaptured++;
        Debug.Log("Captured Turtle");
    }

    void OnBunnyLoses(FunBunny bunny){
        bunny.LoseCondition = true;
        Debug.Log("Bunny Loses");
    }

    void AllCapturedLoss(){
        Debug.Log("Bunny Also Loses");
        bunny.LoseCondition = true;
    }

    public void Update(){

        int turtlesCaptured = 0;
        for(int i = 0; i < turtleFriends.Count;i++) {
            if(turtleFriends[i].Captured){
                turtlesCaptured++;
            }
        }

        if(turtlesCaptured == turtleFriends.Count){
            AllCapturedLoss();
            return;
        }
       
        Collider[] detected = Physics.OverlapBox(bunny.transform.position, new Vector3(0.4f,0.4f,0.4f));
        for(int i = 0; i < detected.Length;i++){

            TurtleFriend tf = detected[i].transform.GetComponent<TurtleFriend>();
            Squid squid = detected[i].transform.GetComponent<Squid>();

            if(squid != null && !bunny.LoseCondition){
                OnBunnyLoses(bunny);
                return;
            }

            if(tf != null && !tf.Saved && !tf.Captured){
                OnSavedTurtle(tf);
            }
        }
    }
	
    IEnumerator Game(){
        yield return StartCoroutine(ShowTitle());
        yield return StartCoroutine(SpawnTurtles());

        while(!bunny.LoseCondition){
            if(turtleFriends.Count <= turtlesSaved){
                level++;
                yield return StartCoroutine(GotoNextLevel());
            }

            yield return null;
        }
       
        yield return StartCoroutine(ShowEnd());
        yield break;
    }

    IEnumerator ShowTitle(){
        while(Input.anyKey == false){
            Debug.Log("waiting");
            yield return null;
        }
        Debug.Log("game starts");
    }

    IEnumerator ShowEnd(){
        yield break;
    }

    IEnumerator GotoNextLevel(){
        
        yield return StartCoroutine(ClearLevel());
        yield return StartCoroutine(ShowSquidMaster());
        yield return StartCoroutine(SpawnTurtles());
        yield return StartCoroutine(SpawnSquids());
        yield break;
    }

    IEnumerator ClearLevel(){
        while(turtleFriends.Count > 0){
            TurtleFriend tf = turtleFriends.Chomp();
            Destroy(tf.gameObject);
        }

        while(turtleFriends.Count > 0){
            Squid s = squids.Chomp();
            Destroy(s.gameObject);
        }
        yield break;
    }

    IEnumerator SpawnTurtles(){
        for(int i =0; i < 3; i++){
            TurtleFriend tf =turtlePrefab.Duplicate(ValidPlacement());
            turtleFriends.Add(tf);
        }
        yield break;
    }

    IEnumerator SpawnSquids(){
        if(level < 1){
            yield break;
        }
        for(int i =0; i < 2; i++){
            Squid s = squidPrefab.Duplicate(ValidPlacement());
            squids.Add(s);
        }
        yield break;
    }

    IEnumerator ShowSquidMaster(){
        if(level >= 2 ){
            Debug.Log("squid master comes in");
        }
        yield break;
    }

    Vector3 ValidPlacement(){

        Vector3 placementPos = Vector3.zero;
        bool found = false;
        Debug.Log("position search begins");
        while(!found){
            
            placementPos = new Vector3(Random.Range(0,9), 0, Random.Range(0,9));
            bool goodPosition = true;

            //check turtles
            for(int i = 0; i < turtleFriends.Count;i++){
                Vector3 tpn = turtleFriends[i].transform.position;
                if(SamePosition(tpn,placementPos)){
                    goodPosition = false;
                }
            }

            //check squids
            for(int i = 0; i < squids.Count;i++){
                Vector3 sqp = squids[i].transform.position;
                if(SamePosition(sqp,placementPos)){
                    goodPosition = false;
                }
            }

            //check bunny
            Vector3 bp = bunny.transform.position;
            if(SamePosition(bp,placementPos)){
                goodPosition = false;
            }

            if(goodPosition){
                found = true;
            }
        }
        Debug.Log("position search ends");
        return placementPos;
    }

    bool SamePosition(Vector3 p,Vector3 op){
        if((int)p.x == (int)op.x 
            && (int)p.z == (int)op.z){
            return true;
        }return false;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(worldWidth/2.0f,0,worldHeight/2.0f),new Vector3(10,1,10));
    }
}
