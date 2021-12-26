using Photon.Pun;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private SpriteRenderer sp;
    private PhotonView view;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject crateSpawnEffect;

    public int spriteNum;
    private GameObject spawnEffect = null;

    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        SetSprite(0);
        SetSpawnEffect();

        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void SetNewCrateRPC(float posX, float posY, int spriteNum)
    {
        transform.position = new Vector3(posX, posY, 0);
        SetSprite(spriteNum);
        SetSpawnEffect();
    }
    
    private void SetSprite(int spriteNum)
    {
        sp.sprite = sprites[spriteNum];
        this.spriteNum = spriteNum;
    }

    private void SetSpawnEffect()
    {
        if (spawnEffect != null)
        {
            Destroy(spawnEffect);
            spawnEffect = null;
        }
        
        spawnEffect = GameObject.Instantiate(crateSpawnEffect, transform.GetChild(0).position, Quaternion.Euler(-90f, 0f, 0f), transform);
    }
}
