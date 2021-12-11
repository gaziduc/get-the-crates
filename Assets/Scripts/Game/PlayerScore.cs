using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    private PhotonView view;
    [SerializeField] private AudioSource crate;

    void Start()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.3f);
        crate.volume = sfxVolume;
        
        PhotonNetwork.LocalPlayer.SetScore(0);
        
        view = GetComponent<PhotonView>();
        if (view.IsMine)
            view.RPC("SetNicknameRPC", RpcTarget.All, view.ViewID, GetComponent<Bot>() ? "Bot" : PhotonNetwork.NickName);
    }

    [PunRPC]
    void SetNicknameRPC(int viewID, string nickname)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(viewID);
        v.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = nickname;
    }
    
    [PunRPC]
    public void IncrementScoreRPC(bool playSound)
    {
        if (playSound)
            crate.Play();
    }
}
