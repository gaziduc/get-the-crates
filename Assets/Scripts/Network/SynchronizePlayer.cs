using Photon.Pun;
using UnityEngine;

public class SynchronizePlayer : MonoBehaviour, IPunObservable
{
    private Rigidbody2D rigidbody;
    private SpriteRenderer sp;
    private Animator anim;
    private PhotonView view;
    private PlayerMovement player;

    private Vector2 networkPosition;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        player = GetComponent<PlayerMovement>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbody.position);
            stream.SendNext(sp.flipX);
            stream.SendNext(anim.GetBool("IsRunning"));
            stream.SendNext(rigidbody.velocity);
        }
        else
        {
            networkPosition = (Vector2) stream.ReceiveNext();
            sp.flipX = (bool) stream.ReceiveNext();
            anim.SetBool("IsRunning", (bool) stream.ReceiveNext());
            rigidbody.velocity = (Vector2) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
            networkPosition += rigidbody.velocity * lag;
        }
    }
    
    public void Update()
    {
        if (!view.IsMine)
        {
            if (Vector2.Distance(rigidbody.position, networkPosition) > 3f) // Teleport if to far
                rigidbody.position = networkPosition;
            else
                rigidbody.position = Vector2.MoveTowards(rigidbody.position, networkPosition, Time.deltaTime * player.moveSpeed);
        }
    }
}
