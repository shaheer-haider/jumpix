using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("Object References")]
    public PlayerController playerController;
    public ChasingEnemy chasingEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            chasingEnemy.catchPlayer();
            PlayerController.Instance.KillPlayer(isDeadByGhost: true);
        }
    }
}
