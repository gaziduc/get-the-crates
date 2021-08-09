using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
   public static Tooltip instance;
   
   [SerializeField] private Text tooltipText;
   [SerializeField] private RectTransform backgroundRectTransform;

   private CanvasGroup canvasGroup;
    
   private void Awake()
   {
      instance = this;

      canvasGroup = GetComponent<CanvasGroup>();
   }

   private void Update()
   {
      if (canvasGroup.alpha > 0f)
         SetPosition();
   }
   
   private void SetPosition()
   {
      transform.position = Input.mousePosition + new Vector3(20f, 0f, 0f);
      if (transform.position.x + backgroundRectTransform.sizeDelta.x + 20f > Screen.width)
         transform.position = new Vector3(Screen.width - backgroundRectTransform.sizeDelta.x - 20f, transform.position.y, transform.position.z);
   }

   public void ShowTooltip(string tooltipString)
   {
      tooltipText.text = tooltipString;
      float paddingTextSize = 10f;
      Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + paddingTextSize * 2f, tooltipText.preferredHeight + paddingTextSize * 2f);
      backgroundRectTransform.sizeDelta = backgroundSize;
      SetPosition();

      canvasGroup.alpha = 0f;
      LeanTween.alphaCanvas(canvasGroup, 1f, 0.2f);
   }
   
   public void HideTooltip()
   {
      LeanTween.alphaCanvas(canvasGroup, 0f, 0.2f);
   }
}
