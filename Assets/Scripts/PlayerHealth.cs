using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public PhotonView view;
    public int initialHealth;
    public int playerNum;
    public int health;
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
        yield return new WaitForSeconds(1.5f);
        
        if (v.IsMine)
            player.playerManager.Respawn(); // Sets position
        
        yield return new WaitForSeconds(1.5f);
        
        player.health = initialHealth;
        gui.SetHealthBar(player.health, player.playerNum);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.transform.localScale = Vector3.one;
        player.GetComponent<PlayerMovement>().canMove = true;
    }

    [PunRPC]
    void HurtRPC(int weaponDamage, int ViewID)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerHealth player = v.GetComponent<PlayerHealth>(); 
        
        player.health -= weaponDamage;
        gui.SetHealthBar(player.health, player.playerNum);

        HealthBarAbovePlayer health = v.GetComponent<HealthBarAbovePlayer>();
        health.SetHealthBar(player.health, gui.gradient);
        
        if (player.health <= 0)
        {
            player.transform.localScale = Vector3.zero;
            player.GetComponent<PlayerMovement>().canMove = false;
            health.SetHealthBar(initialHealth, gui.gradient);
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            
            StartCoroutine(Respawn(player, v));
        }
    }
}
