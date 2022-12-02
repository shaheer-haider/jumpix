using UnityEngine;
using System.Collections;
using SgLib;

public class ParentPlayerController : MonoBehaviour
{
    [Header("Gameplay Config")]
    public float parentPlayerMovingSpeed = 8f;
    public float maxPlayerMovingSpeed = 14f;
    //Moving speed of player
    public int updateValueScore = 30;
    //When you reached this score, moving speed and rotating speed will be increase
    public float movingSpeedAddedValue = 0.5f;
    //Moving Speed will be increase by this value

    [Header("Object References")]
    public GameManager gameManager;
    public PlayerController playerController;

    private Vector3 movingDirection;
    // Moving direction of parent player
    private bool check = true;
    // Use this for initialization
    void Start()
    {

        movingDirection = Vector3.right;
        StartCoroutine(MovingParentPlayer());
    }
	
    // Update is called once per frame
    void Update()
    {
        if ((ScoreManager.Instance.Score != 0) && (ScoreManager.Instance.Score % updateValueScore == 0) && check && gameManager.GameState != GameState.GameOver)
        {
            check = false;

            if (parentPlayerMovingSpeed <= maxPlayerMovingSpeed - movingSpeedAddedValue)
            {
                parentPlayerMovingSpeed += movingSpeedAddedValue; //Increase moving speed
                gameManager.maxNumberOfTrunk += 20; // this is to keep up the faster player speed
                gameManager.trunkExistTime -= 0.05f;
                if (gameManager.trunkExistTime <= 0.05f)
                {
                    gameManager.trunkExistTime = 0.05f;
                }
            }

            StartCoroutine(WaitAndEnableCheck());
        }

    }

    IEnumerator MovingParentPlayer()
    {
        while (true)
        {
            if (playerController.isPlayerRunning && !playerController.isBlockedByBranch && !playerController.isRotatingTrunk && !playerController.touchDisable && gameManager.GameState != GameState.GameOver)
            {
                transform.position = transform.position + movingDirection * parentPlayerMovingSpeed * Time.deltaTime;
            }
            yield return null;
        }
    }

    IEnumerator WaitAndEnableCheck()
    {
        yield return new WaitForSeconds(2f);
        check = true;
    }
}
