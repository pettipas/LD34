using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour {

    public Transform bunnyTitle;
    public Transform startPosition;
    public FunBunny bunnyInstance;
    public float worldWidth = 10.0f;
    public float worldHeight = 10.0f;
    public FunBunny funBunnyPrefab;
    public TurtleFriend turtlePrefab;
    public Squid squidPrefab;
    public int level;
    public int turtlesSaved;
    public static GameLoop Instance;
    public ParticleSystem poof;
    public List<Squid> squidPrefabs = new List<Squid>();
    public List<Transform> spawnPoints = new List<Transform>();

    public List<Squid> squids = new List<Squid>();
    public List<TurtleFriend> turtleFriends = new List<TurtleFriend>();

    void Awake(){
        if(Instance != null){
            Instance = null;
        }
        Instance = this;
    }

    void Start () {
        Time.timeScale = 1.0f;
        bunnyInstance = funBunnyPrefab.Duplicate(startPosition.position);
        bunnyInstance.LoseCondition = false;
        bunnyInstance.GetComponent<Ctrl>().Blocked = false;
        StartCoroutine(Game());
	}

    public Vector3 ChooseAIPosition(List<Vector3> possiblePositions, Vector3 defaultPos, bool chooseHero){
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
        float distanceToBunny = Vector3.Distance(closestToBunny, bunnyInstance.transform.position);

        for(int i = 0; i < possiblePositions.Count; i++){
            Vector3 v = possiblePositions[i];
            float testDistance = Vector3.Distance(v,bunnyInstance.transform.position);
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
            if(testDistance < distanceToTurtle){
                distanceToTurtle = testDistance;
                closestToTurtle = v;
            }
        }

        if(chooseHero){
            return closestToBunny;
        }

        return closestToTurtle;
    }

    void OnSavedTurtle(TurtleFriend saved){
        saved.SaveTurtle();
        saved.Saved = true;
        turtlesSaved++;
    }

    public void OnCapturedTurtle(TurtleFriend captured){
        captured.Captured = true;
        captured.CaptureTurtle();
    }

    void OnBunnyLoses(FunBunny bunny){
        bunnyInstance.LoseCondition = true;
    }

    void AllCapturedLoss(){
        bunnyInstance.LoseCondition = true;
    }

    public void Update(){

        int turtlesCaptured = 0;
        for(int i = 0; i < turtleFriends.Count;i++) {
            if(turtleFriends[i].Captured){
                turtlesCaptured++;
            }
        }

        if(turtlesCaptured == turtleFriends.Count &&  turtlesCaptured != 0){
            AllCapturedLoss();
            return;
        }
       
        Collider[] detected = Physics.OverlapBox(bunnyInstance.transform.position, new Vector3(0.4f,0.4f,0.4f));
        for(int i = 0; i < detected.Length;i++){

            TurtleFriend tf = detected[i].transform.GetComponent<TurtleFriend>();
            Squid squid = detected[i].transform.GetComponent<Squid>();

            if(squid != null && !bunnyInstance.LoseCondition){
                OnBunnyLoses(bunnyInstance);
                return;
            }

            if(tf != null && !tf.Saved && !tf.Captured){
                OnSavedTurtle(tf);
            }
        }
    }
	
    bool AllTurtlesOut{
        get{
            Debug.Log(turtleFriends.FindAll(x=>x.Out == true).Count + " " + turtleFriends.Count);
            return turtleFriends.FindAll(x=>x.Out == true).Count == turtleFriends.Count;
        }
    }

    IEnumerator Game(){
        yield return StartCoroutine(ShowTitle());
        yield return StartCoroutine(SpawnTurtles());

      
        while(!bunnyInstance.LoseCondition){
            if(AllTurtlesOut){  
                level++;
                Time.timeScale += 0.1f;
                yield return StartCoroutine(GotoNextLevel());
            }

            yield return null;
        }
       
        yield return StartCoroutine(ShowEnd());
        yield break;
    }

    IEnumerator ShowTitle(){
        bunnyTitle.GetComponent<Animator>().Play("title_idle",0,0);
        while(Input.anyKey == false){
            yield return null;
        }

        bunnyTitle.GetComponent<Animator>().Play("pop_away",0,0);
        yield return new WaitForSeconds(2.0f);
    }

    IEnumerator ShowEnd(){
        SceneManager.LoadScene("main");
        yield break;
    }

    IEnumerator GotoNextLevel(){
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(ClearLevel());
        yield return StartCoroutine(ShowSquidMaster());
        yield return StartCoroutine(SpawnTurtles());
        yield return StartCoroutine(SpawnSquids());
        yield break;
    }

    IEnumerator ClearLevel(){
        turtlesSaved = 0;

        while(turtleFriends.Count > 0){
            TurtleFriend tf = turtleFriends.Chomp();
            Destroy(tf.gameObject);
        }

        while(squids.Count > 0){
            Squid s = squids.Chomp();
            poof.transform.position = s.transform.position;
            poof.Emit(20);
            Destroy(s.gameObject);
        }

        yield break;
    }

    IEnumerator SpawnTurtles(){
        for(int i =0; i < 3; i++){
            TurtleFriend tf =turtlePrefab.Duplicate(ValidPlacement(4));
            turtleFriends.Add(tf);
        }
        yield break;
    }

    IEnumerator SpawnSquids(){
        if(level < 1){
            yield break;
        }
        if(level >= 4 ){
            for(int i =0; i < 4; i++){
                poof.transform.position = spawnPoints[i].position;
                poof.Emit(30);
                Squid s = squidPrefabs[i].Duplicate(spawnPoints[i].position);
                squids.Add(s);
            }
        }else if(level == 3 ){
            for(int i =0; i < 3; i++){
                poof.transform.position = spawnPoints[i].position;
                poof.Emit(30);
                Squid s = squidPrefabs[i].Duplicate(spawnPoints[i].position);
                squids.Add(s);
            }
        }else if(level == 2 ){
            for(int i =0; i < 2; i++){
                poof.transform.position = spawnPoints[i].position;
                poof.Emit(30);
                Squid s = squidPrefabs[i].Duplicate(spawnPoints[i].position);
                squids.Add(s);
            }
        }else if(level == 1){
            poof.transform.position = spawnPoints[0].position;
            poof.Emit(30);
            Squid s = squidPrefabs[0].Duplicate(spawnPoints[0].position);
            squids.Add(s);
        }
        yield break;
    }

    IEnumerator ShowSquidMaster(){
        if(level == 1 ){
            GetComponent<Animator>().Play("squidmaster_arrives",0,0);
        }
        yield break;
    }

    Vector3 ValidPlacement(float distance){

        Vector3 placementPos = Vector3.zero;
        bool found = false;
        while(!found){
            
            placementPos = new Vector3(Random.Range(0,9), 0, Random.Range(0,9));
            bool goodPosition = true;

            //check turtles
            for(int i = 0; i < turtleFriends.Count;i++){
                Vector3 tpn = turtleFriends[i].transform.position;
                if(SamePosition(tpn,placementPos) && !FarEnoughAway(tpn,placementPos,distance)){
                    goodPosition = false;
                }
            }

            //check squids
            for(int i = 0; i < squids.Count;i++){
                Vector3 sqp = squids[i].transform.position;
                if(SamePosition(sqp,placementPos) && !FarEnoughAway(sqp,placementPos,distance)){
                    goodPosition = false;
                }
            }

            //check bunny
            Vector3 bp = bunnyInstance.transform.position;
            if(SamePosition(bp,placementPos) && !FarEnoughAway(bp,placementPos,distance)){
                goodPosition = false;
            }

            if(goodPosition){
                found = true;
            }
        }
        return placementPos;
    }

    bool FarEnoughAway(Vector3 p, Vector3 op, float distance){
        return Vector3.Distance(p,op) >= distance;
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
