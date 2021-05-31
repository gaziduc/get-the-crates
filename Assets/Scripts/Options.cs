using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    
    public void ToggleOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
}
