using Photon.Pun;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public Vector3 direction;
    public int viewID;
    
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * Time.fixedDeltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player.view.ViewID != viewID)
            {
                if (player.view.IsMine)
                {
                    player.view.RPC("HurtRPC", RpcTarget.All, player.view.ViewID);
                }
                
                GameObject.Destroy(gameObject);
            }
        }
        else
            GameObject.Destroy(gameObject);
    }
}
