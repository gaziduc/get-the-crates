using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] [SerializeField] private string textToShow;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.instance.ShowTooltip(textToShow);
    }
  
    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.HideTooltip();
    }
}
