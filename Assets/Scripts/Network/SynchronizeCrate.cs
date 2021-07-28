using Photon.Pun;
using UnityEngine;

public class SynchronizeCrate : MonoBehaviour, IPunObservable
{
    private Rigidbody2D rigidbody;
    private Vector2 networkPosition;
    private int spriteNum;
    private Crate crate;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        crate = GetComponent<Crate>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbody.position);
            stream.SendNext(crate.spriteNum);
        }
        else
        {
            rigidbody.position = (Vector2) stream.ReceiveNext();
            
            int lastSpriteNum = spriteNum;
            spriteNum = (int) stream.ReceiveNext();
            
            if (lastSpriteNum != spriteNum)
                crate.SetSprite(spriteNum);
        }
    }
}