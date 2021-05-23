using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int score;
    private GuiManager gui;
    public PhotonView view;
    [SerializeField] private AudioSource crate;
    
    private void Start()
    {
        gui = GameObject.FindWithTag("PlayerManager").GetComponent<GuiManager>();
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    void IncrementScoreRPC(int ViewID)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerScore player = v.GetComponent<PlayerScore>();
        PlayerHealth playerHealth = v.GetComponent<PlayerHealth>();

        player.score++;
        gui.SetScore(player.score, playerHealth.playerNum);
        
        crate.Play();
    }
}
