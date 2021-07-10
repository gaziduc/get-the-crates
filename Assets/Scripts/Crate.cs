using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private SpriteRenderer sp;
    [SerializeField] private Sprite[] sprites;
    public int spriteNum = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        SetSprite(0);
    }

    public void SetSprite(int spriteNum)
    {
        sp.sprite = sprites[spriteNum];
        this.spriteNum = spriteNum;
    }
}
