using System;
using Photon.Pun;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public Vector3 direction;
    [SerializeField] private float speed;
    public int viewID;
    public int weaponDamage;
    
    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTrigger(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player.view.ViewID != viewID)
            {
                if (player.view.IsMine)
                    player.view.RPC("HurtRPC", RpcTarget.All, weaponDamage, player.view.ViewID, viewID);

                GameObject.Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Ground"))
            GameObject.Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTrigger(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnTrigger(other);
    }
}
