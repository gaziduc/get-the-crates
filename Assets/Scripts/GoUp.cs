using UnityEngine;

public class GoUp : MonoBehaviour
{
    private TextMesh text;
    
    private void Start()
    {
        text = GetComponent<TextMesh>();
        Destroy(gameObject, 1f);
    }


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime);
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime);
    }
}
