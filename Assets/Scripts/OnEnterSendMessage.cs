using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (inputField.isActiveAndEnabled && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            net.SendChatMessage();
        }
    }
}
