using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    private PhotonView view;
    [SerializeField] private AudioSource crate;
    [SerializeField] private GameObject plusOne;

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
    
    public void PlayScoreSound()
    {
        crate.Play();
    }

    public void AddPlusOne()
    {
        GameObject text = GameObject.Instantiate(plusOne, transform.position + Vector3.up * 2.4f, Quaternion.identity);
        text.GetComponent<TextMesh>().text = "+1";
    }
}
