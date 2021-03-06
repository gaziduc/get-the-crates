using System;
using Photon.Pun;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public Vector3 direction;
    [SerializeField] private float speed;
    public int viewID;
    public int weaponDamage;
    public int weaponNum;
    private int numKilled = 0;
    
    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTrigger(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player.view.ViewID != viewID) // if collider is not the shooter
            {
                if (player.view.IsMine)
                    player.view.RPC("HurtRPC", RpcTarget.All, weaponDamage, player.view.ViewID, viewID, weaponNum);

                if (weaponNum != 4) // if not disk gun
                    GameObject.Destroy(gameObject);
                else
                    numKilled++;
            }
        }
        else if (other.CompareTag("Ground"))
        {
            if (numKilled >= 2)
            {
                PhotonView view = PhotonNetwork.GetPhotonView(viewID);
                if (view.IsMine)
                {
                    GameObject.FindWithTag("PlayerManager").GetComponent<GuiManager>().UnlockTrophyIfNotAchieved("Two in a row", "Kill 2 enemies with 1 shot.");
                }
            }
            GameObject.Destroy(gameObject);
        }
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
