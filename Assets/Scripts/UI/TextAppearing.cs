using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextAppearing : MonoBehaviour
{
    public float timePerLetter = 0.1f;
    
    private string str;
    private Text text;
    private int index = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        text = GetComponent<Text>();
        str = text.text;
        text.text = "";

        StartCoroutine(DisplayText());
    }

    private IEnumerator DisplayText()
    {
        while (index < str.Length)
        {
            text.text += str[index];
            index++;
            yield return new WaitForSeconds(timePerLetter);
        }
    }
}

