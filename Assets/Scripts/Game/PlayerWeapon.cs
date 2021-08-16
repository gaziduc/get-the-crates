using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    public int weaponNum;
    public GameObject[] weaponPrefabs;
    public string[] weaponName = { "Gun", "Machine Gun", "Explosive Gun", "Double Gun" };
    public float[] reloadTime = { 0.25f, 0.15f, 1f, 0.25f };
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
        int lastWeaponNum = weaponNum;
        weaponNum = Random.Range(0, weaponPrefabs.Length);

        // Handle if weapon did not change
        if (weaponNum == lastWeaponNum)
        {
            weaponNum++;
            if (weaponNum >= weaponPrefabs.Length)
                weaponNum = 0;
        }
        
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
