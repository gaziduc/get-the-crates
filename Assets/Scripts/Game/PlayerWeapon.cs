using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    public int weaponNum;
    public GameObject[] weaponPrefabs;
    public string[] weaponName = { "Gun", "Machine Gun", "Explosive Gun", "Double Gun", "Disk Gun" };
    public float[] reloadTime = { 0.25f, 0.15f, 1f, 0.25f, 1.3f };
    private bool[] automatic = { false, true, false, false, false };
    public int[] damage = { 1, 1, 4, 1, 4 };

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
    
    [PunRPC]
    void ShootBulletRPC(float posX, float posY, int weaponNum, float bulletDirection, int viewID)
    {
        GameObject bullet = GameObject.Instantiate(weaponPrefabs[weaponNum],
            new Vector3(posX + bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
        BulletMovement bulletMovement = bullet.GetComponent<BulletMovement>();

        bulletMovement.weaponNum = weaponNum;
        bulletMovement.weaponDamage = damage[weaponNum];
        bulletMovement.direction = new Vector3(bulletDirection, 0, 0);
        bulletMovement.viewID = viewID;

        if (weaponNum == 3) // If Double Gun
        {
            GameObject oppositeBullet = GameObject.Instantiate(weaponPrefabs[weaponNum],
                new Vector3(posX - bulletDirection * 0.2f, posY - 0.2f, 0), Quaternion.identity);
            BulletMovement oppositeBulletMovement = oppositeBullet.GetComponent<BulletMovement>();

            oppositeBulletMovement.weaponNum = weaponNum;
            oppositeBulletMovement.weaponDamage = damage[weaponNum];
            oppositeBulletMovement.direction = new Vector3(-bulletDirection, 0, 0);
            oppositeBulletMovement.viewID = viewID;
        }

        PhotonView playerShooting = PhotonNetwork.GetPhotonView(viewID);
        ReloadBarAbovePlayer reloadBar = playerShooting.GetComponent<ReloadBarAbovePlayer>();
        reloadBar.Shoot(reloadTime[weaponNum]);

        shoot.Play();
    }
}
