using UnityEngine;
using UnityEngine.UI;

public class SetCreditsText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = "David 'Gazi' Ghiassi - 2021 - v" + Application.version;
    }
}
