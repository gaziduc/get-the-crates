using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public bool canMove = true;

    private PhotonView view;

    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private GameObject weaponTextPrefab;
    [SerializeField] private Sprite[] weaponSprites;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 change;
    private bool jump;
    private Vector3 direction;
    private SpriteRenderer sp;
    private Animator anim;
    [SerializeField] private AudioSource shoot;

    private Transform[] feet;
    private PlayerWeapon weapon;

    private KeyCode[] controls;
    private string[] gamepadControls;

    void Start()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.3f);
        shoot.volume = sfxVolume;

        controls = new KeyCode[(int) Options.Controls.NumControls];
        gamepadControls = new string[(int) Options.Controls.NumControls];

        for (int i = 0; i < (int) Options.Controls.NumControls; i++)
        {
            controls[i] = (KeyCode) PlayerPrefs.GetInt(((Options.Controls) i).ToString(),
                (int) System.Enum.Parse(typeof(KeyCode), Options.instance.defaultControls[i]));
            gamepadControls[i] = PlayerPrefs.GetString(((Options.Controls) i).ToString() + "Controller",
                Options.instance.defaultControlsGamepad[i]);
        }

        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        sp.flipX = true;
        anim = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        direction = Vector3.right;
        feet = new Transform[2];
        feet[0] = transform.GetChild(0).GetChild(0);
        feet[1] = transform.GetChild(0).GetChild(1);
        weapon = GetComponent<PlayerWeapon>();
    }

    private void Update()
    {
        if (view.IsMine)
        {
            change = Vector3.zero;

            if (!canMove || !LevelManager.instance.gameStarted)
                return;

            var gamepad = Gamepad.current;

            if (Input.GetKey(controls[(int) Options.Controls.Left]) ||
                (gamepad != null && gamepad[gamepadControls[(int) Options.Controls.Left]].IsPressed()))
            {
                sp.flipX = false;
                if (!anim.GetBool("IsRunning"))
                    anim.SetBool("IsRunning", true);
                change.x = -moveSpeed;
            }
            else if (Input.GetKey(controls[(int) Options.Controls.Right]) || (gamepad != null &&
                                                                              gamepad[
                                                                                      gamepadControls[
                                                                                          (int) Options.Controls.Right]]
                                                                                  .IsPressed()))
            {
                sp.flipX = true;
                if (!anim.GetBool("IsRunning"))
                    anim.SetBool("IsRunning", true);
                change.x = moveSpeed;
            }
            else
            {
                if (anim.GetBool("IsRunning"))
                    anim.SetBool("IsRunning", false);
            }

            if (!change.Equals(Vector3.zero))
                direction = change;


            var jumpControl = gamepad != null ? Gamepad.current[gamepadControls[(int) Options.Controls.Jump]] : null;

            if (IsGrounded() && (Input.GetKeyDown(controls[(int) Options.Controls.Jump]) || (gamepad != null &&
                jumpControl is ButtonControl && ((ButtonControl) jumpControl).wasPressedThisFrame)))
                jump = true;

            var shootControl = gamepad != null ? Gamepad.current[gamepadControls[(int) Options.Controls.Shoot]] : null;

            if (Input.GetKey(controls[(int) Options.Controls.Shoot]) || (gamepad != null && shootControl.IsPressed()))
            {
                if (weapon.isReloaded && (weapon.IsAutomatic() ||
                                          Input.GetKeyDown(controls[(int) Options.Controls.Shoot]) ||
                                          (gamepad != null && shootControl is ButtonControl &&
                                           ((ButtonControl) shootControl).wasPressedThisFrame)))
                {
                    view.RPC("ShootBulletRPC", RpcTarget.All, transform.position.x, transform.position.y,
                        weapon.weaponNum, direction.normalized.x, view.ViewID);
                    weapon.SetReloadBeginning();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            rb.velocity = new Vector2(change.x * Time.fixedDeltaTime, rb.velocity.y);

            if (jump)
            {
                rb.velocity += Vector2.up * 17.5f;
                jump = false;
            }
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(feet[0].position, Vector2.down, 0.1f, platformsLayerMask);
        if (raycastHit2D.collider != null)
            return true;

        raycastHit2D = Physics2D.Raycast(feet[1].position, Vector2.down, 0.1f, platformsLayerMask);
        return raycastHit2D.collider != null;
    }



    [PunRPC]
    void ShootBulletRPC(float posX, float posY, int weaponNum, float bulletDirection, int viewID)
    {
        GameObject bullet = GameObject.Instantiate(weapon.weaponPrefabs[weaponNum],
            new Vector3(posX + bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
        BulletMovement bulletMovement = bullet.GetComponent<BulletMovement>();

        bulletMovement.weaponNum = weaponNum;
        bulletMovement.weaponDamage = weapon.damage[weaponNum];
        bulletMovement.direction = new Vector3(bulletDirection, 0, 0);
        bulletMovement.viewID = viewID;

        if (weaponNum == 3) // If Double Gun
        {
            GameObject oppositeBullet = GameObject.Instantiate(weapon.weaponPrefabs[weaponNum],
                new Vector3(posX - bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
            BulletMovement oppositeBulletMovement = oppositeBullet.GetComponent<BulletMovement>();

            oppositeBulletMovement.weaponNum = weaponNum;
            oppositeBulletMovement.weaponDamage = weapon.damage[weaponNum];
            oppositeBulletMovement.direction = new Vector3(-bulletDirection, 0, 0);
            oppositeBulletMovement.viewID = viewID;
        }

        PhotonView playerShooting = PhotonNetwork.GetPhotonView(viewID);
        ReloadBarAbovePlayer reloadBar = playerShooting.GetComponent<ReloadBarAbovePlayer>();
        reloadBar.Shoot(weapon.reloadTime[weaponNum]);

        shoot.Play();
    }

    [PunRPC]
    void WeaponEffectRPC(int weaponNum, int viewId, bool showText)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(viewId);

        // Set weapon image
        v.transform.GetChild(1).GetChild(2).GetComponent<Image>().sprite = weaponSprites[weaponNum];

        ReloadBarAbovePlayer reloadBar = v.GetComponent<ReloadBarAbovePlayer>();
        reloadBar.SetToReloaded();

        Transform player = v.GetComponent<Transform>();

        // Text above
        if (showText)
        {
            GameObject weaponText = GameObject.Instantiate(weaponTextPrefab, player.position + Vector3.up * 3f, Quaternion.identity);
            weaponText.GetComponent<TextMesh>().text = weapon.weaponName[weaponNum];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (view.IsMine && other.CompareTag("Crate"))
        {
            Crate crate = other.GetComponent<Crate>();
            bool isWeaponCrate = crate.spriteNum == 0;

            // Add 1 to score if it is Get Most Crates mode
            if (LevelManager.instance.winCondition == LevelManager.WinCondition.GetMostCrates)
            {
                view.RPC("IncrementScoreRPC", RpcTarget.All, isWeaponCrate);
                PhotonNetwork.LocalPlayer.AddScore(1);
            }

            // Handle
            if (isWeaponCrate)
            {
                weapon.GetRandom();
                view.RPC("WeaponEffectRPC", RpcTarget.All, weapon.weaponNum, view.ViewID, true);
            }
            else
                view.RPC("HealRPC", RpcTarget.All, view.ViewID);

            // Transfer ownership...
            other.GetComponent<PhotonView>().TransferOwnership(view.Owner);

            // ...to move position
            other.transform.position = SpawnManager.instance.GetCrateNewPosition(other.transform.position);
            crate.SetSprite(Random.Range(0, 8) == 0 ? 1 : 0);
        }
    }
}
