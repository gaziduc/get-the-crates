using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    public int weaponNum;
    public GameObject[] weaponPrefabs;
    private string[] weaponName = {"Gun", "Machine Gun"};
    private float[] reloadTime = { 0f, 0.15f };
    private bool[] automatic = { false, true };

    private float reload;
    public bool isReloaded;
    

    void Update()
    {
        if (!isReloaded)
        {
            reload += Time.deltaTime;
            if (reload >= reloadTime[weaponNum])
                isReloaded = true;
        }
    }

    public void GetRandom()
    {
        weaponNum = Random.Range(0, 2);
        reload = 0f;
        isReloaded = true;
    }

    public bool IsAutomatic()
    {
        return automatic[weaponNum];
    }

    public string GetName()
    {
        return weaponName[weaponNum];
    }
    

    public void SetReloadBeginning()
    {
        isReloaded = false;
        reload = 0f;
    }
}
