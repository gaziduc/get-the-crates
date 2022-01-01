using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trophies : MonoBehaviour
{
    [SerializeField] private GameObject trophiesPanel;
    [SerializeField] private NetworkManager net;
    [SerializeField] private GameObject trophiesPanelContent;
    [SerializeField] private GameObject notLoggedText;

    public static Trophies instance;

    private const float c = 35f / 255f;

    void Start()
    {
        instance = this;
    }
    
    public void ToggleTrophies()
    {
        Options.instance.CloseOptions();
        
        if (trophiesPanel.activeInHierarchy)
        {
            LeanTween.scale(trophiesPanel, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => { trophiesPanel.SetActive(false); });
        }
        else
        {
            trophiesPanel.SetActive(true);
            trophiesPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(trophiesPanel, Vector3.one, 0.2f).setEaseOutBack();

            if (net.trophiesUnlocked != null && net.trophiesUnlocked.Length > 0)
            {
                notLoggedText.SetActive(false);
                
                for (int i = 0; i < trophiesPanelContent.transform.childCount; i++)
                {
                    // If achieved
                    if (net.trophiesUnlocked[i])
                        trophiesPanelContent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
                    else
                        trophiesPanelContent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(c, c, c, 1f);
                }
            }
            else
            {
                notLoggedText.SetActive(true);
                
                for (int i = 0; i < trophiesPanelContent.transform.childCount; i++)
                    trophiesPanelContent.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(c, c, c, 1f);
            }
        }
    }

    public void CloseTrophies()
    {
        if (trophiesPanel.activeInHierarchy)
            LeanTween.scale(trophiesPanel, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => { trophiesPanel.SetActive(false); });
    }
}
