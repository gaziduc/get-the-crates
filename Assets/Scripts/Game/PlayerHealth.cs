using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public PhotonView view;
    public int initialHealth;
    public int health;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject deathByDiskEffect;
    [SerializeField] private GameObject healthEffect;
    [SerializeField] private GameObject weaponTextPrefab;

    private void Start()
    {
        health = initialHealth;
        view = GetComponent<PhotonView>();
    }

    private IEnumerator Respawn(PlayerHealth player, PhotonView v)
    {
        yield return new WaitForSeconds(1.5f);

        if (v.IsMine)
        {
            player.transform.position = SpawnManager.instance.GetRespawnPos();
            
            PlayerWeapon w = player.GetComponent<PlayerWeapon>();
            w.ResetWeapon();
            view.RPC("WeaponEffectRPC", RpcTarget.All, w.weaponNum, v.ViewID, false);
        }
            
        
        yield return new WaitForSeconds(1.5f);
        
        player.health = initialHealth;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.transform.localScale = Vector3.one;
        
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            
        // If not a bot
        if (playerMovement)
            playerMovement.canMove = true;
        else
            player.GetComponent<Bot>().canMove = true;
    }

    [PunRPC]
    void HurtRPC(int weaponDamage, int ViewID, int ShooterID, int weaponNum)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerHealth player = v.GetComponent<PlayerHealth>(); 
        
        player.health -= weaponDamage;
        HealthBarAbovePlayer health = v.GetComponent<HealthBarAbovePlayer>();
        health.SetHealthBar(player.health);
        
        if (player.health <= 0)
        {
            GameObject go = GameObject.Instantiate(weaponNum != 4 ? deathEffect : deathByDiskEffect, transform.position, Quaternion.identity);
            AudioSource audio = go.GetComponent<AudioSource>();
            if (audio)
                audio.volume = LevelManager.instance.sfxVolume;
            
            player.transform.localScale = Vector3.zero;

            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            
            // If not a bot
            if (playerMovement)
                playerMovement.canMove = false;
            else
                player.GetComponent<Bot>().canMove = false;
            
            health.SetHealthBar(initialHealth);
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;


            if (LevelManager.instance.winCondition == LevelManager.WinCondition.KillMostPlayers)
            {
                PhotonView shooterView = PhotonNetwork.GetPhotonView(ShooterID);
                PlayerScore shooterScore = shooterView.GetComponent<PlayerScore>();
                shooterScore.IncrementScoreRPC(true);

                Bot bot = shooterView.GetComponent<Bot>();
                if (bot)
                    bot.score++;
                else
                    shooterView.Owner.AddScore(1);
            }
            
            StartCoroutine(Respawn(player, v));
        }
    }
    
    [PunRPC]
    void HealRPC(int ViewID)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerHealth player = v.GetComponent<PlayerHealth>(); 
        
        player.health = initialHealth;
        HealthBarAbovePlayer health = v.GetComponent<HealthBarAbovePlayer>();
        health.SetHealthBar(player.health);

        Transform tf = v.GetComponent<Transform>();
        
        // Text above
        GameObject weaponText = GameObject.Instantiate(weaponTextPrefab, tf.position + Vector3.up * 3f, Quaternion.identity);
        weaponText.GetComponent<TextMesh>().text = "Health";
        
        GameObject go = GameObject.Instantiate(healthEffect, tf.position + Vector3.up * 2.4f, Quaternion.identity);
        go.GetComponent<AudioSource>().volume = LevelManager.instance.sfxVolume;
    }
}
