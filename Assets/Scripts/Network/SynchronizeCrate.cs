using Photon.Pun;
using UnityEngine;

public class SynchronizeCrate : MonoBehaviour, IPunObservable
{
    private Transform transform;
    private Vector2 networkPosition;
    private int spriteNum;
    private Crate crate;
    
    private void Start()
    {
        transform = GetComponent<Transform>();
        crate = GetComponent<Crate>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(crate.spriteNum);
        }
        else
        {
            transform.position = (Vector3) stream.ReceiveNext();
            
            int lastSpriteNum = spriteNum;
            spriteNum = (int) stream.ReceiveNext();
            
            if (lastSpriteNum != spriteNum)
                crate.SetSprite(spriteNum);
        }
    }
}