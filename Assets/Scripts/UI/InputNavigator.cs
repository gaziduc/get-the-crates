using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputNavigator : MonoBehaviour
{
    private EventSystem system;

    void Start()
    {
        system = EventSystem.current;
    }
    
    // Update is called once per frame
    void Update()
    {
        Selectable next = null;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (!Keyboard.current.shiftKey.isPressed)
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            else
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
        }


        if (next != null)
        {
            InputField inputfield = next.GetComponent<InputField>();
            if (inputfield != null)
                inputfield.OnPointerClick(new PointerEventData(system)); // if it's an input field, also set the text caret */

            system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
        }
    }
}