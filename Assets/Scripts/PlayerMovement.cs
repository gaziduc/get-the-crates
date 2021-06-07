using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public bool canMove = true;

    private PhotonView view;
    
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private GameObject weaponTextPrefab;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 change;
    private bool jump;
    private Vector3 direction;
    private SpriteRenderer sp;
    private Animator anim;
    [SerializeField] private AudioSource shoot;
    private GuiManager gui;

    private Transform[] feet;

    private PlayerWeapon weapon;

    private InstantiatePlayerOnStart instantiate;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        sp.flipX = true;
        anim = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        direction = Vector3.right;
        gui = GameObject.FindWithTag("PlayerManager").GetComponent<GuiManager>();
        feet = new Transform[2];
        feet[0] = transform.GetChild(0).GetChild(0);
        feet[1] = transform.GetChild(0).GetChild(1);
        weapon = GetComponent<PlayerWeapon>();
        instantiate = GameObject.FindWithTag("PlayerManager").GetComponent<InstantiatePlayerOnStart>();
    }

    private void Update()
    {
        if (view.IsMine)
        {
            change = Vector3.zero;
            
            if (!canMove)
                return;
            
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                sp.flipX = false;
                if (!anim.GetBool("IsRunning"))
                    anim.SetBool("IsRunning", true);
                change.x = -moveSpeed;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
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

            if (IsGrounded() && Input.GetKeyDown(KeyCode.UpArrow))
                jump = true;

            if (Input.GetKey(KeyCode.Space))
            {
                if (weapon.isReloaded && (weapon.IsAutomatic() || Input.GetKeyDown(KeyCode.Space)))
                {
                    view.RPC("ShootBulletRPC", RpcTarget.All, transform.position.x, transform.position.y, weapon.weaponNum, direction.normalized.x, view.ViewID);
                    weapon.SetReloadBeginning();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            transform.Translate(change * Time.fixedDeltaTime);

            if (jump)
            {
                rb.AddForce(Vector2.up * 800);
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
        GameObject bullet = GameObject.Instantiate(weapon.weaponPrefabs[weaponNum],new Vector3(posX + bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
        BulletMovement bulletMovement = bullet.GetComponent<BulletMovement>();
        
        bulletMovement.direction = new Vector3(bulletDirection, 0, 0);
        bulletMovement.viewID = viewID;
        
        shoot.Play();
    }
    
    [PunRPC]
    void PauseRPC()
    {
        gui.pausePanel.SetActive(!gui.pausePanel.activeInHierarchy);
        canMove = !gui.pausePanel.activeInHierarchy;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (view.IsMine && other.gameObject.CompareTag("Crate"))
        {
            weapon.GetRandom();
            
            GameObject weaponText = GameObject.Instantiate(weaponTextPrefab, transform.position + Vector3.up * 2.5f, Quaternion.identity);
            GameObject.Destroy(weaponText, 1f);

            weaponText.GetComponent<TextMesh>().text = weapon.GetName();
            
            
            view.RPC("IncrementScoreRPC", RpcTarget.All, view.ViewID);
            
            // Transfert ownership...
            other.gameObject.GetComponent<PhotonView>().TransferOwnership(view.Owner);
            
            // ...to move position
            other.transform.position = instantiate.GetCrateNewPosition(other.transform.position);
            other.rigidbody.velocity = Vector2.zero;
        }
    }
}
