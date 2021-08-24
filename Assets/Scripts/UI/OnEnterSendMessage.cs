using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OnEnterSendMessage : MonoBehaviour
{
    private InputField inputField;
    [SerializeField] private NetworkManager net;
    
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.isActiveAndEnabled && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            net.SendChatMessage();
        }
    }
}
