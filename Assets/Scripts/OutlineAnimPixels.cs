using UnityEngine;
using UnityEngine.UI;

public class OutlineAnimPixels : MonoBehaviour
{
    private Texture2D texture;
    private RawImage image;
    private RectTransform rect;
    private RectTransform panelRect;
    [SerializeField] private int direction;
    private Vector3[] panelCorners;
    private float speed = 100f;
    private Vector2Int res;
    
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<RawImage>();
        panelRect = transform.parent.parent.GetComponent<RectTransform>();
        
        CreateNewTexture();

        panelCorners = new Vector3[4];
        panelRect.GetWorldCorners(panelCorners);

        res = new Vector2Int(Screen.width, Screen.height);
    }

    private void CreateNewTexture()
    {
        texture = new Texture2D((int) rect.rect.width, (int) rect.rect.height);

        float widthMiddle = texture.width / 2f;
        
        for (int i = 0; i < widthMiddle; i++)
        {
            float c = i / widthMiddle;
            
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, new Color(c, c, c, c));
        }

        for (int i = (int) widthMiddle; i < texture.width; i++)
        {
            float c = (1 - i + texture.width) / widthMiddle;
            
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, new Color(c, c, c, c));
        }

        texture.Apply();
        
        image.texture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        // If resolution changed, re-get panel corners
        if (res.x != Screen.width || res.y != Screen.height)
        {
            panelRect.GetWorldCorners(panelCorners);
            
            res.x = Screen.width;
            res.y = Screen.height;
        }
        
        
        rect.Translate(Vector3.right * speed * direction * Time.deltaTime);
        
        Vector3[] cornersArray = new Vector3[4];
        rect.GetWorldCorners(cornersArray);
        
        
        if (cornersArray[0].x <= panelCorners[0].x || cornersArray[2].x >= panelCorners[2].x)
        {
            direction = -direction;
        }
    }
}
