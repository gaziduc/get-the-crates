using System.Collections;
using UnityEngine;

public class GoUp : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one * 0.1f, 0.2f).setEaseOutBack();
        StartCoroutine(DestoryCoroutine());

    }

    private IEnumerator DestoryCoroutine()
    {
        yield return new WaitForSeconds(1.2f);
        LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => Destroy(gameObject));
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 0.5f);
    }
}
