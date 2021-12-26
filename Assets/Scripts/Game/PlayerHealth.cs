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
    public bool invincible = false;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject deathByDiskEffect;
    [SerializeField] private GameObject healthEffect;
    [SerializeField] private GameObject weaponTextPrefab;
    [SerializeField] private GameObject spawnPrefab;

    private void Start()
    {
        health = initialHealth;
        view = GetComponent<PhotonView>();
        
        // Spawn particle effects
        GameObject.Instantiate(spawnPrefab, transform.position, Quaternion.identity);
    }

    private IEnumerator Respawn(PlayerHealth player, PhotonView v)
    {
        yield return new WaitForSeconds(2f);

        if (v.IsMine)
        {
            player.transform.position = SpawnManager.instance.GetRespawnPos();
            
            PlayerWeapon w = player.GetComponent<PlayerWeapon>();
            w.ResetWeapon();
            view.RPC("WeaponEffectRPC", RpcTarget.All, w.weaponNum, v.ViewID, false);
        }
            
        
        yield return new WaitForSeconds(2f);
        
        player.health = initialHealth;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.transform.localScale = Vector3.one;
        player.invincible = true;
        
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            
        // If not a bot
        if (playerMovement)
            playerMovement.canMove = true;
        else
            player.GetComponent<Bot>().canMove = true;
        
        // Spawn particle effects
        GameObject.Instantiate(spawnPrefab, transform.position, Quaternion.identity);

        SpriteRenderer sp = player.GetComponent<SpriteRenderer>();

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.25f);
            sp.enabled = false;
            yield return new WaitForSeconds(0.25f);
            sp.enabled = true;
        }
        
        player.invincible = false;
    }

    [PunRPC]
    void HurtRPC(int weaponDamage, int ViewID, int ShooterID, int weaponNum)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(ViewID);
        PlayerHealth player = v.GetComponent<PlayerHealth>();

        if (player.invincible)
            return;
        
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
                Bot bot = shooterView.GetComponent<Bot>();
                
                if (shooterView.IsMine && bot == null)
                {
                    shooterView.Owner.AddScore(1);
                    PlayerScore shooterScore = shooterView.GetComponent<PlayerScore>();
                    shooterScore.PlayScoreSound();
                    shooterScore.AddPlusOne();
                }
                else if (bot)
                    bot.score++;
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
