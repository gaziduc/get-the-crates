using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    private PhotonView view;
    [SerializeField] private AudioSource crate;

    void Start()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.4f);
        crate.volume = sfxVolume;
        
        PhotonNetwork.LocalPlayer.SetScore(0);
        
        view = GetComponent<PhotonView>();
        if (view.IsMine)
            view.RPC("SetNicknameRPC", RpcTarget.All, view.ViewID, PhotonNetwork.NickName);
    }

    [PunRPC]
    void SetNicknameRPC(int viewID, string nickname)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(viewID);
        v.transform.GetChild(1).GetComponent<TextMesh>().text = nickname;
    }
    
    [PunRPC]
    public void IncrementScoreRPC(int viewID, int score, bool playSound)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(viewID);
        ScoreAbovePlayer hud = v.GetComponent<ScoreAbovePlayer>();
        hud.SetScoreHud(score);

        if (playSound)
            crate.Play();
    }
}
