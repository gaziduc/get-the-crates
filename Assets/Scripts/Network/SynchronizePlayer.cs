using Photon.Pun;
using UnityEngine;

public class SynchronizePlayer : MonoBehaviour, IPunObservable
{
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    private Animator anim;
    private PhotonView view;
    [SerializeField] private bool isBot = false;
    private Bot bot;

    private Vector2 networkPosition;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            stream.SendNext(rb.position);
            stream.SendNext(sp.flipX);
            stream.SendNext(anim.GetBool("IsRunning"));
            stream.SendNext(rb.velocity);
            if (isBot)
                stream.SendNext(bot.score);
        }
        else
        {
            networkPosition = (Vector2) stream.ReceiveNext();
            sp.flipX = (bool) stream.ReceiveNext();
            weaponSpriteRenderer.flipX = !sp.flipX; // Weapon;
            anim.SetBool("IsRunning", (bool) stream.ReceiveNext());
            rb.velocity = (Vector2) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
            networkPosition += rb.velocity * lag;

            if (isBot)
                bot.score = (int) stream.ReceiveNext();
        }
    }
    
    public void Update()
    {
        if (!view.IsMine)
        {
            if (Vector2.Distance(rb.position, networkPosition) > 2.5f) // Teleport if to far
                rb.position = networkPosition;
            else
                rb.position = Vector2.MoveTowards(rb.position, networkPosition, Time.deltaTime * 2f);
        }
    }
}
