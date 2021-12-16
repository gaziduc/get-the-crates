using UnityEngine;

public class Quit : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            #if UNITY_WEBGL
                if (Screen.fullScreen)
                    Screen.fullScreen = false;
            #endif

            Application.Quit();
        #endif
    }
}
