using Photon.Pun;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int score;
    private GuiManager gui;
    [SerializeField] private AudioSource crate;

    private void Start()
    {
        gui = GameObject.FindWithTag("PlayerManager").GetComponent<GuiManager>();
    }

    [PunRPC]
    void IncrementScoreRPC(int ViewID)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerScore player = v.GetComponent<PlayerScore>();
        PlayerHealth playerHealth = v.GetComponent<PlayerHealth>();
        PlayerWeapon weapon = v.GetComponent<PlayerWeapon>();
        
        player.score++;
        gui.SetScore(player.score, playerHealth.playerNum);

        crate.Play();
    }

    [PunRPC]
    void EndRPC()
    {
        gui.ShowEnd();
    }
}
