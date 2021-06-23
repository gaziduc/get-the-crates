using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    public int weaponNum;
    public GameObject[] weaponPrefabs;
    private string[] weaponName = { "Gun", "Machine Gun", "Explosive Gun", "Double Gun" };
    public float[] reloadTime = { 0.3f, 0.15f, 1f, 0.3f };
    private bool[] automatic = {false, true, false, false};
    public int[] damage = { 1, 1, 4, 1 };

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
        weaponNum = Random.Range(0, weaponPrefabs.Length);
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
