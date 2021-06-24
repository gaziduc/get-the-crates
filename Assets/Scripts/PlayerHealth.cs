using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public PhotonView view;
    public int initialHealth;
    public int health;
    private InstantiatePlayerOnStart playerManager;

    [SerializeField] private GameObject deathEffect;

    private void Start()
    {
        health = initialHealth;
        view = GetComponent<PhotonView>();
        playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<InstantiatePlayerOnStart>();
    }

    private IEnumerator Respawn(PlayerHealth player, PhotonView v)
    {
        yield return new WaitForSeconds(1.5f);
        
        if (v.IsMine)
            player.playerManager.Respawn(); // Sets position
        
        yield return new WaitForSeconds(1.5f);
        
        player.health = initialHealth;
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
        HealthBarAbovePlayer health = v.GetComponent<HealthBarAbovePlayer>();
        health.SetHealthBar(player.health);
        
        if (player.health <= 0)
        {
            GameObject.Instantiate(deathEffect, transform.position, Quaternion.identity);
            
            player.transform.localScale = Vector3.zero;
            player.GetComponent<PlayerMovement>().canMove = false;
            health.SetHealthBar(initialHealth);
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            
            StartCoroutine(Respawn(player, v));
        }
    }
}
