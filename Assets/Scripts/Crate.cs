using Photon.Pun;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private void OnCollision(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerScore playerScore = other.gameObject.GetComponent<PlayerScore>();
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerScore.view.RPC("IncrementScoreRPC", RpcTarget.All, playerScore.view.ViewID);

            transform.position = InstantiatePlayerOnStart.GetCrateNewPosition();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
       OnCollision(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        OnCollision(other);
    }
}
