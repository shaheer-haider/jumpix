using UnityEngine;
using System.Collections;

public class ChasingEnemy : MonoBehaviour
{

    [Header("Object References")]
    public GameManager gameManager;
    public ParentPlayerController parentPlayerController;
    public PlayerController playerController;

    private float ghostSpeed;
    private float defaultGhostSpeed;
    private float initialDeltaX;
    Transform playerTransform;
    public Animator animator;
    float ghostSpeedFactor;
    private bool isDancing;

    public static ChasingEnemy Instance;

    // Use this for initialization
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        isDancing = true;
        animator.SetBool("isDancing", true);
        ghostSpeedFactor = CameraController.camSpeedFactor;
        playerTransform = playerController.transform;
        initialDeltaX = playerTransform.position.x - transform.position.x;
        StartCoroutine(MoveGhost());
    }

    IEnumerator MoveGhost()
    {
        while (true)
        {
            if (gameManager.GameState != GameState.GameOver && playerController.isPlayerRunning && !playerController.isRotatingTrunk && !playerController.touchDisable)
            {
                if (isDancing)
                {
                    isDancing = false;
                    animator.SetBool("isDancing", false);
                }
                defaultGhostSpeed = parentPlayerController.parentPlayerMovingSpeed * ghostSpeedFactor;
                float deltaX = playerTransform.position.x - transform.position.x;
                float diff = deltaX - initialDeltaX;

                if (diff > 0.5f)
                {
                    ghostSpeed = defaultGhostSpeed + diff;
                }
                else
                {
                    ghostSpeed = defaultGhostSpeed;
                }

                transform.position += Vector3.right * ghostSpeed * Time.deltaTime;
            }
            yield return null;
        }
    }

    // stop the ghost courotines when player dies
    public void catchPlayer()
    {
        StopCoroutine(MoveGhost());
        animator.SetTrigger("Catch");
    }

    public void laughing()
    {
        animator.SetTrigger("Laughing");
        animator.SetBool("isDancing", true);
    }
}
