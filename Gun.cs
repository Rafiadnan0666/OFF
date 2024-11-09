using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public enum GunType { Pistol, MachineGun }
    public GunType gunType;

    public Transform aimPosition; 
    public Transform gunAimPosition; 
    public Transform cameraAimPosition; 
    public Transform bulletTip; 

    public GameObject muzzleFlashPrefab; 
    public GameObject Bullet;
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

    private Vector3 originalGunPosition;
    private Vector3 originalCameraPosition;

    void Start()
    {
        mainCamera = Camera.main;
        normalFOV = mainCamera.fieldOfView;
        originalGunPosition = transform.localPosition; 
        originalCameraPosition = mainCamera.transform.localPosition; 
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isEquipped)
        {
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                isFullAuto = !isFullAuto;
            }

          
            //if (Input.GetMouseButtonDown(1)) 
            //{
            //    isScoped = !isScoped;
            //    StartCoroutine(ScopeEffect(isScoped));
            //}

   
            if (isFullAuto && Input.GetMouseButton(0)) 
            {
                Fire();
            }
            else if (Input.GetMouseButtonDown(0)) 
            {
                Fire();
            }

            
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }
        }
    }

    public void Equip(bool equip)
    {
        isEquipped = equip;
    }
            
    void Fire()
    {
        if (currentAmmo > 0)
        {
           
            Instantiate(muzzleFlashPrefab, bulletTip.position, bulletTip.rotation);
            Instantiate(Bullet, bulletTip.position, bulletTip.rotation);

       
            StartCoroutine(cameraShake.Shake(0.1f, 0.3f)); 

          
            currentAmmo--;
            UpdateAmmoUI();
        }
    }

    //IEnumerator ScopeEffect(bool enable)
    //{
    //    if (enable)
    //    {
            
    //        while (mainCamera.fieldOfView > scopedFOV || mainCamera.transform.position != cameraAimPosition.position)
    //        {
    //            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, scopedFOV, aimSpeed * Time.deltaTime);
    //            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraAimPosition.position, aimSpeed * Time.deltaTime);
    //            transform.localPosition = Vector3.Lerp(transform.localPosition, gunAimPosition.localPosition, aimSpeed * Time.deltaTime); 
    //            yield return null;
    //        }
    //    }
    //    else
    //    {
         
    //        while (mainCamera.fieldOfView < normalFOV || mainCamera.transform.localPosition != originalCameraPosition)
    //        {
    //            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, aimSpeed * Time.deltaTime);
    //            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, originalCameraPosition, aimSpeed * Time.deltaTime);
    //            transform.localPosition = Vector3.Lerp(transform.localPosition, originalGunPosition, aimSpeed * Time.deltaTime); 
    //            yield return null;
    //        }
    //    }
    //}

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

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }
}
