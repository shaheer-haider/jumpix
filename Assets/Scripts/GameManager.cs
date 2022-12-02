using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SgLib;




public enum GameState
{
    Prepare,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event System.Action<GameState, GameState> GameStateChanged = delegate { };

    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        private set
        {
            if (value != _gameState)
            {
                GameState oldState = _gameState;
                _gameState = value;

                GameStateChanged(_gameState, oldState);
            }
        }
    }

    [SerializeField]
    private GameState _gameState = GameState.Prepare;

    public static int GameCount
    {
        get { return _gameCount; }
        private set { _gameCount = value; }
    }

    private static int _gameCount = 0;

    [Header("Gameplay Config")]
    public int maxBranchScale = 3;
    //Max length to scale branch
    public float waitTimeMoveAndDestroyTrunk = 0.1f;
    //How fast trunk is moved
    public float waitTimeReducedValue = 0.01f;
    public float minWaitTime = 0.02f;
    //How fast trunk is moved when game over
    public float trunkExistTime = 0.1f;
    public int maxNumberOfTrunk = 60;
    public float trunkMovingSpeed = 20f;
    //Moving speed of trunk
    public float trunkRotatingSpeed = 500f;
    //How fast trunk rotating
    public int changeRotateDirectionScore = 20;
    //When you reached this score, trunk will rotate by opposite direction

    [Range(0f, 1f)]
    public float coinFrequency;
    //Probability to create gold

    [Range(0f, 1f)]
    public float threeBranchFrequency;
    //Probability to create three branchs

    [Range(0f, 1f)]
    public float twoBranchFrequency;
    //Probability to create gold two branchs

    [Range(0f, 1f)]
    public float oneBranchFrequency;
    //Probability to create gold one branch

    [Header("Object Preferences")]
    public PlayerController playerController;
    public GameObject parentPlayer;
    public GameObject firstTrunk;
    public GameObject trunkPrefab;
    public GameObject branchPrefab;
    public GameObject coin;

    // Use this for initialization
    private List<GameObject> listTrunk = new List<GameObject>();
    private GameObject currentTrunk;
    private GameObject currentBranchPrefab;
    private GameObject[] temp;
    private Vector3 position;
    private bool stopMoveTrunk = false;
    private int count = 0;

    public float lastGameOverTime;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void OnEnable()
    {
        PlayerController.PlayerDie += PlayerController_PlayerDie;
    }

    void OnDisable()
    {
        PlayerController.PlayerDie -= PlayerController_PlayerDie;
    }

    void Start()
    {
        GameState = GameState.Prepare;
        listTrunk.Add(firstTrunk); //Add first trunk for list
        position = firstTrunk.transform.position + Vector3.right; //Create position to create trunk

        for (int i = 0; i < 15; i++) //Create 15 first trunks
        {
            currentTrunk = (GameObject)Instantiate(trunkPrefab, position, Quaternion.identity);
            listTrunk.Add(currentTrunk);
            currentTrunk.transform.SetParent(gameObject.transform);
            position = currentTrunk.transform.position + Vector3.right;
        }

        StartCoroutine(MoveAllTrunk());
        StartCoroutine(RandomTrunkAndBranch());
    }

    public void PlayerController_PlayerDie()
    {
        GameOver();
    }

    public void StartGame()
    {
        GameState = GameState.Playing;
    }

    public void GameOver()
    {
           
        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
        lastGameOverTime = Time.time;
        GameCount++;
        GameState = GameState.GameOver;
    }

    public void RestartGame(float delay = 0)
    {
        StartCoroutine(CRRestartGame(delay));
    }

    IEnumerator CRRestartGame(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator RandomTrunkAndBranch()
    {
        while (true)
        {
            yield return null;
            if (GameState != GameState.GameOver && playerController.isPlayerRunning && !playerController.isRotatingTrunk && !playerController.touchDisable)
            {
                temp = GameObject.FindGameObjectsWithTag("Trunk");
                if (temp.Length < maxNumberOfTrunk)
                {
                    count++;
                    currentTrunk = (GameObject)Instantiate(trunkPrefab, position, Quaternion.identity);
                    currentTrunk.transform.SetParent(gameObject.transform);
                    listTrunk.Add(currentTrunk);
                    position = currentTrunk.transform.position + Vector3.right;

                    if (count % 2 == 0) // If count devisible by 2, random branch (this if condition to make game not impossible)
                    {
                        //Create position
                        Vector3 forwarPos = currentTrunk.transform.position + new Vector3(0, 0, 0.5f);
                        Vector3 backPos = currentTrunk.transform.position + new Vector3(0, 0, -0.5f);
                        Vector3 upPos = currentTrunk.transform.position + new Vector3(0, 0.5f, 0);
                        Vector3 downPos = currentTrunk.transform.position + new Vector3(0, -0.5f, 0);

                        //Create first list
                        List<Vector3> firstListPositionOfBranch = new List<Vector3>();

                        //Add position for first list
                        firstListPositionOfBranch.Add(forwarPos);
                        firstListPositionOfBranch.Add(backPos);
                        firstListPositionOfBranch.Add(upPos);
                        firstListPositionOfBranch.Add(downPos);


                        //Create last list
                        List<Vector3> lastListPositionOfBranch = new List<Vector3>();


                        /*While the length of last list still less than 3, that mean last list 
                          have not enough, add position from first list for it
                        */
                        while (lastListPositionOfBranch.Count < 3)
                        {
                            //Random index to add for last list
                            int indexOfFirstListPositionOfBranch = UnityEngine.Random.Range(0, firstListPositionOfBranch.Count);

                            //Check index already exist in last list, if == true, index has exist and else, add index for last list
                            if (lastListPositionOfBranch.Contains(firstListPositionOfBranch[indexOfFirstListPositionOfBranch]) == false)
                            {
                                lastListPositionOfBranch.Add(firstListPositionOfBranch[indexOfFirstListPositionOfBranch]);
                            }
                        }

                        //Now create the branch
                        int probabilityToCreateBranch = UnityEngine.Random.Range(0, 4);
                        if (probabilityToCreateBranch == 0)//Now create 3 branchs
                        {
                            float probabilityToCreateThreeBranch = UnityEngine.Random.Range(0f, 1f); //Create probability

                            if (probabilityToCreateThreeBranch <= threeBranchFrequency)
                            {
                                for (int i = 0; i < lastListPositionOfBranch.Count; i++) //This loop run 3 times and create 3 branchs from 3 position of last list
                                {
                                    if (lastListPositionOfBranch[i] == forwarPos)
                                    {
                                        currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[i], Quaternion.identity);
                                    }
                                    else if (lastListPositionOfBranch[i] == backPos)
                                    {
                                        currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[i], Quaternion.Euler(180, 0, 0));
                                    }
                                    else if (lastListPositionOfBranch[i] == upPos)
                                    {
                                        currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[i], Quaternion.Euler(270, 0, 0));
                                    }
                                    else if (lastListPositionOfBranch[i] == downPos)
                                    {
                                        currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[i], Quaternion.Euler(90, 0, 0));
                                    }
                                    currentBranchPrefab.transform.SetParent(currentTrunk.transform);
                                }
                            }

                        }
                        else if (probabilityToCreateBranch == 1)//Now create two branch
                        {
                            float probabilityToCreateTwoBranch = UnityEngine.Random.Range(0f, 1f); //Create probability

                            if (probabilityToCreateTwoBranch <= twoBranchFrequency)
                            {
                                while (lastListPositionOfBranch.Count > 1) //If the length of last list equal 1, that mean there is 2 branchs already created, stop create cube 
                                {
                                    int indexToCreateBranch = UnityEngine.Random.Range(0, lastListPositionOfBranch.Count); //Random position for branch you create
                                    if (lastListPositionOfBranch[indexToCreateBranch] != new Vector3(0.0f, 0.0f, 0.0f)) //Check position not equals to Vector3(0,0,0), then create create branch, else --> continue loop
                                    {
                                        if (lastListPositionOfBranch[indexToCreateBranch] == forwarPos)
                                        {
                                            currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.identity);
                                        }
                                        else if (lastListPositionOfBranch[indexToCreateBranch] == backPos)
                                        {
                                            currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.Euler(180, 0, 0));
                                        }
                                        else if (lastListPositionOfBranch[indexToCreateBranch] == upPos)
                                        {
                                            currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.Euler(270, 0, 0));
                                        }
                                        else if (lastListPositionOfBranch[indexToCreateBranch] == downPos)
                                        {
                                            currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.Euler(90, 0, 0));
                                        }
                                        currentBranchPrefab.transform.SetParent(currentTrunk.transform);
                                        lastListPositionOfBranch.RemoveAt(indexToCreateBranch); //After create branch, remove position that you take from list to make next position not equal with this position 
                                    }
                                }
                            }

                        }
                        else if (probabilityToCreateBranch == 2)//Now create 1 branch
                        {
                            float probabilityToCreateOneBranch = UnityEngine.Random.Range(0f, 1f); //Create probability

                            if (probabilityToCreateOneBranch <= oneBranchFrequency)
                            {
                                int indexToCreateBranch = UnityEngine.Random.Range(0, lastListPositionOfBranch.Count);//Random position for branch you create
                                if (lastListPositionOfBranch[indexToCreateBranch] == forwarPos)
                                {
                                    currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.identity);
                                }
                                else if (lastListPositionOfBranch[indexToCreateBranch] == backPos)
                                {
                                    currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.Euler(180, 0, 0));
                                }
                                else if (lastListPositionOfBranch[indexToCreateBranch] == upPos)
                                {
                                    currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.Euler(270, 0, 0));
                                }
                                else if (lastListPositionOfBranch[indexToCreateBranch] == downPos)
                                {
                                    currentBranchPrefab = (GameObject)Instantiate(branchPrefab, lastListPositionOfBranch[indexToCreateBranch], Quaternion.Euler(90, 0, 0));
                                }
                                currentBranchPrefab.transform.SetParent(currentTrunk.transform);
                            }

                        }
                        else //Create gold
                        {
                            float probabilityToCreateGold = UnityEngine.Random.Range(0f, 1f);
                            int indexToCreateGold = UnityEngine.Random.Range(0, lastListPositionOfBranch.Count); //Random position for gold you create
                            GameObject currentGold;
                            if (probabilityToCreateGold <= coinFrequency)
                            {
                                if (lastListPositionOfBranch[indexToCreateGold] == forwarPos)
                                {
                                    currentGold = (GameObject)Instantiate(coin, lastListPositionOfBranch[indexToCreateGold] + new Vector3(0, 0, 1f), Quaternion.identity);
                                    currentGold.transform.SetParent(currentTrunk.transform);
                                }
                                else if (lastListPositionOfBranch[indexToCreateGold] == backPos)
                                {
                                    currentGold = (GameObject)Instantiate(coin, lastListPositionOfBranch[indexToCreateGold] + new Vector3(0, 0, -1f), Quaternion.identity);
                                    currentGold.transform.SetParent(currentTrunk.transform);
                                }
                                else if (lastListPositionOfBranch[indexToCreateGold] == upPos)
                                {
                                    currentGold = (GameObject)Instantiate(coin, lastListPositionOfBranch[indexToCreateGold] + new Vector3(0, 1f, 0), Quaternion.identity);
                                    currentGold.transform.SetParent(currentTrunk.transform);
                                }
                                else if (lastListPositionOfBranch[indexToCreateGold] == downPos)
                                {
                                    currentGold = (GameObject)Instantiate(coin, lastListPositionOfBranch[indexToCreateGold] + new Vector3(0, -1f, 0), Quaternion.identity);
                                    currentGold.transform.SetParent(currentTrunk.transform);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator MoveAllTrunk()
    {
        int i = 0;
        while (true)
        {
            while (listTrunk != null && playerController != null && playerController.isPlayerRunning)//Start to move trunk destroy it
            {
                if (GameState != GameState.GameOver)
                {
                    float xDistance = parentPlayer.transform.position.x - listTrunk[i].transform.position.x;
                    if (xDistance > 1)
                    {
                        StartCoroutine(MoveOneTrunk(listTrunk[i]));
                        i++;
                    }
                }
                else //Game over, move all trunk on scene view
                {
                    waitTimeMoveAndDestroyTrunk = minWaitTime;
                    StartCoroutine(MoveOneTrunk(listTrunk[i]));
                    if (i == listTrunk.Count - 1)
                    {
                        stopMoveTrunk = true;
                        break;
                    }
                    i++;
                }
                yield return new WaitForSeconds(waitTimeMoveAndDestroyTrunk);
            }
            yield return null;
        }
    }

    IEnumerator MoveOneTrunk(GameObject trunk)
    {
        float startTime = Time.time;
        if (GameState == GameState.GameOver)
        {
            trunkExistTime = 0;
        }
        while ((Time.time - startTime < trunkExistTime) && !stopMoveTrunk)
        {
            trunk.transform.position = trunk.transform.position + Vector3.left * trunkMovingSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(trunk);
    }
}
