using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkText : MonoBehaviour
{
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1.0f;
    [SerializeField] private float speed = 2f;

    private bool isGoingDown = false;
    private Text text;
    public bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            if (isGoingDown)
            {
                if (text.color.a > minAlpha)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - speed * Time.deltaTime);
                else
                    isGoingDown = false;
            }
            else
            {
                if (text.color.a < maxAlpha)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + speed * Time.deltaTime);
                else
                    isGoingDown = true;
            }
        }
    }

    public void EnableBlink()
    {
        isEnabled = true;
        isGoingDown = true;
    }

    public void DisableBlink()
    {
        isEnabled = false;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 255);
    }
}
