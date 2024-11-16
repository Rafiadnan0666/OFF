using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public enum GunType { Pistol, MachineGun, Sniper }
    public GunType gunType;

    public Transform aimPosition;
    public Transform originalPosition;
    public Transform bulletTip;
    public Camera scopeCamera;
    public Camera playerCamera;

    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

    public CameraShake cameraShake;

    public float aimSpeed = 5f;
    public float scopedFOV = 30f;
    private float normalFOV;
    private Camera mainCamera;

    public int maxAmmo = 100;
    public int currentAmmo;
    public Text ammoText;

    public bool isEquipped = false;
    private bool isScoped = false;
    public bool isFullAuto = false;
    public bool isSniper = false;

    private Vector3 originalGunPosition;

    void Start()
    {
        mainCamera = playerCamera;
        normalFOV = mainCamera.fieldOfView;
        originalGunPosition = transform.localPosition;
        currentAmmo = maxAmmo;
        scopeCamera.gameObject.SetActive(false);
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isEquipped)
        {
            playerCamera.gameObject.SetActive(true);

            // Toggle scope state when right-clicking
            //if (Input.GetMouseButtonDown(1))
            //{
            //    isScoped = !isScoped;
            //    StartCoroutine(AimGun(isScoped));
            //}

            //Ensure the correct cameras are activated based on scope state
            //if (isScoped)
            //{
            //    playerCamera.gameObject.SetActive(true);
            //    scopeCamera.gameObject.SetActive(true);
            //}
            //else
            //{
            //    playerCamera.gameObject.SetActive(true);
            //    scopeCamera.gameObject.SetActive(false);
            //}

            // Fire logic
            if (isFullAuto && Input.GetMouseButton(0))
            {
                Fire();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Fire();
            }

            // Reload logic
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator AimGun(bool enable)
    {
        Vector3 targetPosition = enable ? aimPosition.localPosition : originalGunPosition;
        float targetFOV = enable ? scopedFOV : normalFOV;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * aimSpeed;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, elapsed);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, elapsed);
            yield return null;
        }
    }

    void Fire()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();

  
            Instantiate(bulletPrefab, bulletTip.position, bulletTip.rotation);


            if (audioSource != null && fireSound != null)
            {
                audioSource.PlayOneShot(fireSound);
            }

            if (muzzleFlashPrefab != null)
            {
                Instantiate(muzzleFlashPrefab, bulletTip.position, bulletTip.rotation);
            }


            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0f, 0f));
            }
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }


    public void Equip(bool equip)
    {
        isEquipped = equip;

        if (equip)
        {
      
            Debug.Log("Gun equipped");
        }
        else
        {
       
            Debug.Log("Gun unequipped");
        }

      
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }
    }

    IEnumerator Reload()
    {
        float reloadTime = 2f;
        float spinSpeed = 360;
        float elapsedTime = 0f;

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        while (elapsedTime < reloadTime)
        {
            transform.Rotate(Vector3.right, spinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }
}
