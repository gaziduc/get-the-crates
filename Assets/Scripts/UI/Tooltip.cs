using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
   public static Tooltip instance;
   
   [SerializeField] private Text tooltipText;
   [SerializeField] private RectTransform backgroundRectTransform;
    
   private void Awake()
   {
      instance = this;
      HideTooltip();
   }

   private void Update()
   {
      transform.position = Input.mousePosition;
   }

   public void ShowTooltip(string tooltipString)
   {
      gameObject.SetActive(true);

      tooltipText.text = tooltipString;
      float paddingTextSize = 4f;
      Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + paddingTextSize * 2f, tooltipText.preferredHeight + paddingTextSize * 2f);
      backgroundRectTransform.sizeDelta = backgroundSize;
   }

   public void HideTooltip()
   {
      gameObject.SetActive(false);
   }
}
