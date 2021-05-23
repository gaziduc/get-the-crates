using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public PhotonView view;
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

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            gui.SetNicknameText(PhotonNetwork.PlayerList[i].NickName, i);
            gui.SetMaxHealth(initialHealth, i);
        }

        transform.GetChild(1).GetComponent<TextMesh>().text = PhotonNetwork.PlayerList[playerNum].NickName;
    }

    private IEnumerator Respawn(PlayerHealth player, PhotonView v)
    {
        yield return new WaitForSeconds(3f);
        player.transform.localScale = Vector3.one;
        player.health = initialHealth;
        gui.SetHealthBar(player.health, player.playerNum);
        
        if (v.IsMine)
            player.playerManager.Respawn();
    }

    [PunRPC]
    void HurtRPC(int ViewID)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerHealth player = v.GetComponent<PlayerHealth>(); 
        
        player.health--;
        gui.SetHealthBar(player.health, player.playerNum);
        
        if (player.health <= 0)
        {
            StartCoroutine(Respawn(player, v));
            player.transform.localScale = Vector3.zero;
        }
    }
}
