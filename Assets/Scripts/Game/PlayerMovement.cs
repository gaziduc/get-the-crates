using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 change;
    private bool jump;
    private Vector3 direction;
    private SpriteRenderer sp;
    private Animator anim;
    
    private Transform[] feet;
    private PlayerWeapon weapon;

    private KeyCode[] controls;
    private string[] gamepadControls;

    private Joystick joystick;
    private Button shootButton;
    private bool mobileShooting = false;
    private bool mobileShootingDown = false;

    void Start()
    {
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
        GameObject joystickGameobject = GameObject.FindWithTag("Joystick");
        if (joystickGameobject != null)
            joystick = joystickGameobject.GetComponent<Joystick>();
        GameObject shootButtonGameobject = GameObject.FindWithTag("ShootButton");
        if (shootButtonGameobject != null)
        {
            shootButton = shootButtonGameobject.GetComponent<Button>();
            
            // Pointer down
            EventTrigger.Entry downEntry = new EventTrigger.Entry();
            downEntry.eventID = EventTriggerType.PointerDown;
            downEntry.callback.AddListener(arg => { OnShootPointerDown((PointerEventData) arg); });
            shootButton.GetComponent<EventTrigger>().triggers.Add(downEntry);
            
            // Pointer up
            EventTrigger.Entry upEntry = new EventTrigger.Entry();
            upEntry.eventID = EventTriggerType.PointerUp;
            upEntry.callback.AddListener(arg => { OnShootPointerUp((PointerEventData) arg); });
            shootButton.GetComponent<EventTrigger>().triggers.Add(upEntry);
        }
            
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
                (gamepad != null && gamepad[gamepadControls[(int) Options.Controls.Left]].IsPressed()) ||
                (joystick != null && joystick.Horizontal <= -0.2f))
            {
                sp.flipX = false;
                if (!anim.GetBool("IsRunning"))
                    anim.SetBool("IsRunning", true);
                change.x = -moveSpeed;
            }
            else if (Input.GetKey(controls[(int) Options.Controls.Right])
                     || (gamepad != null && gamepad[gamepadControls[(int) Options.Controls.Right]].IsPressed()) ||
                     (joystick != null && joystick.Horizontal >= 0.2f))
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
                jumpControl is ButtonControl && ((ButtonControl) jumpControl).wasPressedThisFrame) || 
                (joystick != null && joystick.Vertical >= 0.7f)))
                jump = true;

            var shootControl = gamepad != null ? Gamepad.current[gamepadControls[(int) Options.Controls.Shoot]] : null;

            if (Input.GetKey(controls[(int) Options.Controls.Shoot]) || (gamepad != null && shootControl.IsPressed())
                || (shootButton != null && (mobileShooting || mobileShootingDown)))
            {
                if (weapon.isReloaded && (weapon.IsAutomatic() ||
                                          Input.GetKeyDown(controls[(int) Options.Controls.Shoot]) ||
                                          (gamepad != null && shootControl is ButtonControl &&
                                          ((ButtonControl) shootControl).wasPressedThisFrame) ||
                                          (shootButton != null && mobileShootingDown)))
                {
                    mobileShootingDown = false;
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
                rb.velocity = Vector2.up * 17.5f;
                jump = false;
            }
        }
    }

    private void OnShootPointerDown(PointerEventData data)
    {
        mobileShooting = true;
        mobileShootingDown = true;
    }

    private void OnShootPointerUp(PointerEventData data)
    {
        mobileShooting = false;
        mobileShootingDown = false;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(feet[0].position, Vector2.down, 0.1f, platformsLayerMask);
        if (raycastHit2D.collider != null)
            return true;

        raycastHit2D = Physics2D.Raycast(feet[1].position, Vector2.down, 0.1f, platformsLayerMask);
        return raycastHit2D.collider != null;
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
                PlayerScore playerScore = GetComponent<PlayerScore>();
                playerScore.PlayScoreSound();
                playerScore.AddPlusOne();
                PhotonNetwork.LocalPlayer.AddScore(1);
            }

            // Handle
            if (isWeaponCrate)
            {
                weapon.GetRandom();
                view.RPC("WeaponEffectRPC", RpcTarget.All, weapon.weaponNum, view.ViewID, true);
            }
            else
            {
                if (GetComponent<PlayerHealth>().health == 1)
                    GameObject.FindWithTag("PlayerManager").GetComponent<GuiManager>().UnlockTrophyIfNotAchieved("Useful health", "Pick-up a health crate when 1 HP.");
                
                view.RPC("HealRPC", RpcTarget.All, view.ViewID);
            }
                

            // Transfer ownership...
            PhotonView crateView = other.GetComponent<PhotonView>();
            crateView.TransferOwnership(view.Owner);

            // ...to move position
            Vector3 newPos = SpawnManager.instance.GetCrateNewPosition(other.transform.position);
            crateView.RPC("SetNewCrateRPC", RpcTarget.All, newPos.x, newPos.y, Random.Range(0, 8) == 0 ? 1 : 0);
        }
    }
}
