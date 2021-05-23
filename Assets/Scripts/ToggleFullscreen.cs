using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFullscreen : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ToggleFullscreenWindowed();
        }
    }

    public void ToggleFullscreenWindowed()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
