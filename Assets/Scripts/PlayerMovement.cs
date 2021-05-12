using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    [SerializeField] private float bulletSpeed;
    
    private PhotonView view;
    
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private GameObject bulletPrefab;
    
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private Vector3 change;
    private bool jump;
    private Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        view = GetComponent<PhotonView>();
        direction = Vector3.right;
    }

    private void Update()
    {
        if (view.IsMine)
        {
            change = Vector3.zero;

            if (Input.GetKey(KeyCode.LeftArrow))
                change.x = -moveSpeed;
            else if (Input.GetKey(KeyCode.RightArrow))
                change.x = moveSpeed;

            if (!change.Equals(Vector3.zero))
                direction = change * 5f;

            if (IsGrounded() && Input.GetKeyDown(KeyCode.UpArrow))
                jump = true;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                view.RPC("ShootBulletRPC", RpcTarget.All, transform.position.x, transform.position.y, direction.normalized.x);
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
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down , 0.1f, platformsLayerMask);
        return raycastHit2D.collider != null;
    }

    [PunRPC]
    void ShootBulletRPC(float posX, float posY, float bulletDirection)
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab,new Vector3(posX + bulletDirection, posY, 0), Quaternion.identity);
        bullet.GetComponent<BulletMovement>().direction = new Vector3(bulletDirection * bulletSpeed, 0, 0);
    }
}
