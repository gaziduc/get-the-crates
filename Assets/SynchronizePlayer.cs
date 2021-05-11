using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SynchronizePlayer : MonoBehaviour, IPunObservable
{
    private Rigidbody2D rigidbody;
    private PhotonView view;
    private PlayerMovement player;

    private Vector2 networkPosition;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        player = GetComponent<PlayerMovement>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbody.position);
            stream.SendNext(rigidbody.rotation);
            stream.SendNext(rigidbody.velocity);
        }
        else
        {
            networkPosition = (Vector2) stream.ReceiveNext();
            rigidbody.rotation = (float) stream.ReceiveNext();
            rigidbody.velocity = (Vector2) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
            networkPosition += rigidbody.velocity * lag;
        }
    }
    
    public void FixedUpdate()
    {
        if (!view.IsMine)
        {
            rigidbody.position = Vector2.MoveTowards(rigidbody.position, networkPosition, Time.fixedDeltaTime * player.moveSpeed);
        }
    }
}
