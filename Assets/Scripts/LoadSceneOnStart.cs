using Photon.Pun;
using UnityEngine;

public class LoadSceneOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LoadLevel("Level");
    }
}
