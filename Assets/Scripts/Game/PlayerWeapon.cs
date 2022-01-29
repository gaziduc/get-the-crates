using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    public enum Weapon
    {
        Gun = 0,
        MachineGun = 1,
        ExplosiveGun = 2,
        DoubleGun = 3,
        DiskGun = 4,
        Shotgun = 5,
    }
    
    public int weaponNum;
    public GameObject[] weaponPrefabs;
    public string[] weaponName = { "Gun", "Machine Gun", "Explosive Gun", "Double Gun", "Disk Gun", "Shotgun" };
    public float[] reloadTime = { 0.25f, 0.15f, 1f, 0.25f, 1.3f, 0.55f };
    private bool[] automatic = { false, true, false, false, false, false };
    public int[] damage = { 1, 1, 4, 1, 4, 1 };

    private float reload;
    public bool isReloaded;

    [SerializeField] private GameObject weaponTextPrefab;
    [SerializeField] private Sprite[] weaponSprites;
    [SerializeField] private AudioSource shoot;
    [SerializeField] private AudioSource weaponSound;
    
    private void Start()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.3f);
        shoot.volume = sfxVolume;
        weaponSound.volume = sfxVolume;
    }

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

    public void ResetWeapon()
    {
        weaponNum = 0;
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
    
    [PunRPC]
    void WeaponEffectRPC(int weaponNum, int viewId, bool showText)
    {
        PhotonView v = PhotonNetwork.GetPhotonView(viewId);

        // Set weapon image
        v.transform.GetChild(1).GetChild(2).GetComponent<Image>().sprite = weaponSprites[weaponNum];

        ReloadBarAbovePlayer reloadBar = v.GetComponent<ReloadBarAbovePlayer>();
        reloadBar.SetToReloaded();

        Transform player = v.GetComponent<Transform>();

        // Text above
        if (showText)
        {
            GameObject weaponText = GameObject.Instantiate(weaponTextPrefab, player.position + Vector3.up * 3f, Quaternion.identity);
            weaponText.GetComponent<TextMesh>().text = weaponName[weaponNum];
            
            weaponSound.Play();
        }
    }

    private void SetBulletProps(BulletMovement bullet, int weaponIndex, Vector3 bulletDirection, int viewID)
    {
        bullet.weaponNum = weaponIndex;
        bullet.weaponDamage = damage[weaponNum];
        bullet.direction = bulletDirection;
        bullet.viewID = viewID;
    }
    
    [PunRPC]
    void ShootBulletRPC(float posX, float posY, int weaponIndex, float bulletDirection, int viewID)
    {
        switch (weaponIndex)
        {
            case (int) Weapon.Gun:
            case (int) Weapon.DiskGun:
            case (int) Weapon.MachineGun:
            case (int) Weapon.ExplosiveGun:
                GameObject bullet = GameObject.Instantiate(weaponPrefabs[weaponIndex],
                    new Vector3(posX + bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
                SetBulletProps(bullet.GetComponent<BulletMovement>(), weaponIndex, new Vector3(bulletDirection, 0, 0), viewID);
                break;
            case (int) Weapon.DoubleGun:
                GameObject bullet1 = GameObject.Instantiate(weaponPrefabs[weaponIndex],
                    new Vector3(posX + bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
                SetBulletProps(bullet1.GetComponent<BulletMovement>(), weaponIndex, new Vector3(bulletDirection, 0, 0), viewID);
                GameObject bullet2 = GameObject.Instantiate(weaponPrefabs[weaponIndex],
                    new Vector3(posX - bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
                SetBulletProps(bullet2.GetComponent<BulletMovement>(), weaponIndex, new Vector3(-bulletDirection, 0, 0), viewID);
                break;
            case (int) Weapon.Shotgun:
                for (int i = -2; i <= 2; i++)
                {
                    GameObject shotgunBullet = GameObject.Instantiate(weaponPrefabs[weaponIndex],
                        new Vector3(posX + bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);

                    Vector3 direction = new Vector3();
                    direction.x = bulletDirection * Mathf.Cos(i * 5 * Mathf.Deg2Rad);
                    direction.y = bulletDirection * Mathf.Sin(i * 5 * Mathf.Deg2Rad);
                    
                    SetBulletProps(shotgunBullet.GetComponent<BulletMovement>(), weaponIndex, direction, viewID);
                }
                break;
        }

        PhotonView playerShooting = PhotonNetwork.GetPhotonView(viewID);
        ReloadBarAbovePlayer reloadBar = playerShooting.GetComponent<ReloadBarAbovePlayer>();
        reloadBar.Shoot(reloadTime[weaponIndex]);

        shoot.Play();
    }
}
