using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private Vector3 change;
    private bool jump;
    private PhotonView view;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        view = GetComponent<PhotonView>();
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

            if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
                jump = true;
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            transform.Translate(change * Time.fixedDeltaTime);

            if (jump)
            {
                rb.AddForce(Vector2.up * 700);
                jump = false;
            }
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down , 0.1f, platformsLayerMask);
        return raycastHit2D.collider != null;
    }
}
