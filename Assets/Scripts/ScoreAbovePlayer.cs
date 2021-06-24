using UnityEngine;

public class ScoreAbovePlayer : MonoBehaviour
{
    private TextMesh text;
    
    // Start is called before the first frame update
    void Start()
    {
        text = transform.GetChild(3).GetComponent<TextMesh>();
    }

    public void SetScoreHud(int score)
    {
        text.text = score.ToString();
    }
}
