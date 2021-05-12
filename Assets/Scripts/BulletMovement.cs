using System;
using Photon.Pun;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public Vector3 direction;
    
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * Time.fixedDeltaTime);
    }
}
