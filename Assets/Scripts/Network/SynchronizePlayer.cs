using Photon.Pun;
using UnityEngine;

public class SynchronizePlayer : MonoBehaviour, IPunObservable
{
    private Rigidbody2D rigidbody;
    private SpriteRenderer sp;
    private Animator anim;
    private PhotonView view;
    [SerializeField] private bool isBot = false;
    private Bot bot;

    private Vector2 networkPosition;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (isBot)
            bot = GetComponent<Bot>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbody.position);
            stream.SendNext(sp.flipX);
            stream.SendNext(anim.GetBool("IsRunning"));
            stream.SendNext(rigidbody.velocity);
            if (isBot)
                stream.SendNext(bot.score);
        }
        else
        {
            networkPosition = (Vector2) stream.ReceiveNext();
            sp.flipX = (bool) stream.ReceiveNext();
            anim.SetBool("IsRunning", (bool) stream.ReceiveNext());
            rigidbody.velocity = (Vector2) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
            networkPosition += rigidbody.velocity * lag;

            if (isBot)
                bot.score = (int) stream.ReceiveNext();
        }
    }
    
    public void Update()
    {
        if (!view.IsMine)
        {
            if (Vector2.Distance(rigidbody.position, networkPosition) > 2.5f) // Teleport if to far
                rigidbody.position = networkPosition;
            else
                rigidbody.position = Vector2.MoveTowards(rigidbody.position, networkPosition, Time.deltaTime * 2f);
        }
    }
}
