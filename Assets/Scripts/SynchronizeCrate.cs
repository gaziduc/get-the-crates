using Photon.Pun;
using UnityEngine;

public class SynchronizeCrate : MonoBehaviour, IPunObservable
{
    private Rigidbody2D rigidbody;
    private Vector2 networkPosition;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbody.position);
        }
        else
        {
            rigidbody.position = (Vector2) stream.ReceiveNext();
        }
    }
}