using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private BoxCollider2D collider;
    private bool isGrounded;
    private Vector3 change;
    private bool jump;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        change = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
            change.x = -moveSpeed;
        else if (Input.GetKey(KeyCode.RightArrow))
            change.x = moveSpeed;

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
            jump = true;
    }

    private void FixedUpdate()
    {
        transform.Translate(change * Time.fixedDeltaTime);

        if (jump)
        {
            rb.AddForce(Vector2.up * 700);
            jump = false;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down , 0.1f, platformsLayerMask);
        return raycastHit2D.collider != null;
    }
}
