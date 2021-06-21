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
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel("Level");
    }
}
