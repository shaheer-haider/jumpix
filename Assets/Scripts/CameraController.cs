using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("How fast camera moves, the faster the more difficult game becomes")]
    [Range(0.1f, 1)]
    public static float camSpeedFactor = 0.8f;

    [Header("Object References")]
    public GameManager gameManager;
    public ParentPlayerController parentPlayerController;
    public PlayerController playerController;
    public Color[] colors;

    private float cameraSpeed;
    private float defaultCameraSpeed;
    private float initialDeltaX;
    Transform playerTransform;

    // Use this for initialization
    void Start()
    {
        int indexBackgroundColor = Random.Range(0, colors.Length);
        GetComponent<Camera>().backgroundColor = colors[indexBackgroundColor];
        playerTransform = playerController.transform;
        initialDeltaX = playerTransform.position.x - transform.position.x;
        StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {        
        while (true)
        {
            if (gameManager.GameState != GameState.GameOver && playerController.isPlayerRunning && !playerController.isRotatingTrunk && !playerController.touchDisable)
            {
                defaultCameraSpeed = parentPlayerController.parentPlayerMovingSpeed * camSpeedFactor;
                float deltaX = playerTransform.position.x - transform.position.x;
                float diff = deltaX - initialDeltaX;

                if (diff > 0.5f)
                {
                    cameraSpeed = defaultCameraSpeed + diff;
                }
                else
                {
                    cameraSpeed = defaultCameraSpeed;
                }

                transform.position += Vector3.right * cameraSpeed * Time.deltaTime;
            }
            yield return null;
        }
    }
}
