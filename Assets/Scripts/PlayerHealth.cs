using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PhotonView view;
    public int initialHealth;
    public int playerNum;
    private int health;
    private InstantiatePlayerOnStart playerManager;
    private GuiManager gui;
    
    private void Start()
    {
        health = initialHealth;
        playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<InstantiatePlayerOnStart>();
        gui = GameObject.FindWithTag("PlayerManager").GetComponent<GuiManager>();
        
        view = GetComponent<PhotonView>();

        if (view.CreatorActorNr == PhotonNetwork.PlayerList[0].ActorNumber)
            playerNum = 0;
        else
            playerNum = 1;

        for (int i = 0; i < 2; i++)
        {
            gui.SetNicknameText(PhotonNetwork.PlayerList[i].NickName, i);
            gui.SetHealthText(initialHealth, i);
        }

        transform.GetChild(1).GetComponent<TextMesh>().text = "P" + (playerNum + 1);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Bullet"))
        {
            if (view.IsMine)
            {
                view.RPC("Hurt", RpcTarget.All, view.ViewID);
            }
                
            
            GameObject.Destroy(other.gameObject);
        }
    }

    private IEnumerator Respawn(PlayerHealth player)
    {
        yield return new WaitForSeconds(3f);
        player.transform.localScale = Vector3.one;
        player.health = initialHealth;
        gui.SetHealthText(player.health, player.playerNum);
        player.playerManager.Respawn();
    }

    [PunRPC]
    void Hurt(int ViewID)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerHealth player = v.GetComponent<PlayerHealth>(); 
        
        player.health--;
        gui.SetHealthText(player.health, player.playerNum);
        
        if (player.health <= 0)
        {
            StartCoroutine(Respawn(player));
            player.transform.localScale = Vector3.zero;
        }
    }
}
