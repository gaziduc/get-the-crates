using System.Collections;
using Photon.Pun;
using UnityEngine;

public class LoadSceneOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(LoadLevelCoroutine());
    }

    private IEnumerator LoadLevelCoroutine()
    {
        yield return new WaitForSeconds(0.65f);
        PhotonNetwork.LoadLevel(PlayerPrefs.GetString("LastLevelSize", "Small"));
    }
}
