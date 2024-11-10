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

    public GameObject muzzleFlashPrefab;
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
            if (Input.GetMouseButtonDown(1))
            {
                isScoped = !isScoped;
                StartCoroutine(AimGun(isScoped));
            }

            // Ensure the correct cameras are activated based on scope state
            if (isScoped)
            {
                playerCamera.gameObject.SetActive(true);
                scopeCamera.gameObject.SetActive(true);
            }
            else
            {
                playerCamera.gameObject.SetActive(true);
                scopeCamera.gameObject.SetActive(false);
            }

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
            Vector3 direction = (isScoped ? scopeCamera.transform : playerCamera.transform).forward;
            if (isFullAuto)
            {
                float spread = 0.05f;
                direction += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
            }

            RaycastHit hit;
            if (Physics.Raycast((isScoped ? scopeCamera.transform : playerCamera.transform).position, direction, out hit, 100f))
            {
                Debug.Log($"Hit: {hit.collider.name}");
            }

            if (muzzleFlashPrefab != null)
            {
                Instantiate(muzzleFlashPrefab, bulletTip.position, bulletTip.rotation);
            }
            else
            {
                Debug.LogWarning("Muzzle flash prefab is not assigned.");
            }

            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.1f, 0.3f));
            }
            else
            {
                Debug.LogWarning("Camera shake is not assigned.");
            }

            currentAmmo--;
            UpdateAmmoUI();
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }


    IEnumerator Reload()
    {
        float reloadTime = 2f;
        float spinSpeed = 7200f;
        float elapsedTime = 0f;

        while (elapsedTime < reloadTime)
        {
            transform.Rotate(Vector3.right, spinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    public void Equip(bool equip)
    {
        isEquipped = equip;

        if (equip)
        {
            // Logic for equipping the gun
            Debug.Log("Gun equipped");
        }
        else
        {
            // Logic for unequipping the gun
            Debug.Log("Gun unequipped");
        }

        // Ensure the player camera stays active even if the parent is set inactive
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }
    }

    IEnumerator ScopeEffect(bool enable)
    {
        float targetFOV = enable ? scopedFOV : normalFOV;
        float startFOV = mainCamera.fieldOfView;
        float elapsed = 0f;

        // Detach playerCamera temporarily to prevent deactivation
        if (enable)
        {
            playerCamera.transform.SetParent(null, true);
        }

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * aimSpeed;
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed);
            yield return null;
        }

        // Re-parent playerCamera after scoping ends
        if (!enable)
        {
            playerCamera.transform.SetParent(transform, true);
        }
    }
    
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }
}
