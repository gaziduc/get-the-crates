using Photon.Pun;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private SpriteRenderer sp;
    private PhotonView view;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject crateSpawnEffect;
    public int spriteNum = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        SetSprite(0);
        SetSpawnEffectRPC();

        view = GetComponent<PhotonView>();
    }

    public void SetSprite(int spriteNum)
    {
        sp.sprite = sprites[spriteNum];
        this.spriteNum = spriteNum;
    }

    public void SetSpawnEffect()
    {
        view.RPC("SetSpawnEffectRPC", RpcTarget.All);
    }
    
    [PunRPC]
    private void SetSpawnEffectRPC()
    {
        GameObject.Instantiate(crateSpawnEffect, transform.GetChild(0).position, Quaternion.Euler(-90f, 0f, 0f), transform);
    }
}
