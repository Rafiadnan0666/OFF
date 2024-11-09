using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    public GameObject flashlightSlot;

    public Transform spawnPoint;

    public Image slot1Image;
    public Image slot2Image;
    public Image slot3Image;
    public Image flashlightSlotImage;

    public Text slot1Text;
    public Text slot2Text;
    public Text slot3Text;
    public Text flashlightSlotText;

    private GameObject[] slots;
    private GameObject[] storedItems;
    private Text[] slotTexts;
    private Image[] slotImages;
    private GameObject currentItem;

    private GameObject equippedFlashlight;

    void Start()
    {
        slots = new GameObject[] { slot1, slot2, slot3, flashlightSlot };
        storedItems = new GameObject[slots.Length];
        slotTexts = new Text[] { slot1Text, slot2Text, slot3Text, flashlightSlotText };
        slotImages = new Image[] { slot1Image, slot2Image, slot3Image, flashlightSlotImage };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) AccessSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AccessSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AccessSlot(2);
        if (Input.GetKeyDown(KeyCode.F)) ToggleFlashlight(); // Toggle flashlight

        if (Input.GetKeyDown(KeyCode.Q)) DropCurrentItem();
    }

    public void StoreItem(GameObject item)
    {
        if (item.CompareTag("Flashlight"))
        {
            StoreFlashlight(item);
        }
        else
        {
            for (int i = 0; i < storedItems.Length - 1; i++) // Exclude the flashlight slot
            {
                if (storedItems[i] == null)
                {
                    storedItems[i] = item;
                    item.SetActive(false);

                    slotTexts[i].text = item.name;
                    slotImages[i].sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;

                    break;
                }
            }
        }
    }

    private void StoreFlashlight(GameObject flashlight)
    {
        if (equippedFlashlight != null)
        {
            equippedFlashlight.SetActive(false);
            StoreItem(equippedFlashlight);
        }

        equippedFlashlight = flashlight;
        flashlight.SetActive(false);

        flashlightSlotText.text = flashlight.name;
        flashlightSlotImage.sprite = flashlight.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    public void ToggleFlashlight()
    {
        if (equippedFlashlight != null)
        {
            Flashlight flashlight = equippedFlashlight.GetComponent<Flashlight>();
            flashlight?.ToggleLight();
        }
    }

    public void AccessSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= storedItems.Length) return;

        if (storedItems[slotIndex] != null)
        {
            if (currentItem != null)
            {
                StoreItem(currentItem);  // Store the current equipped item back in the inventory
                currentItem.transform.parent = null; // Detach from spawn point
            }

            currentItem = storedItems[slotIndex];
            storedItems[slotIndex] = null;

            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;  // Disable physics for the equipped item
            }

            currentItem.SetActive(true);

            // Make the item a child of the spawn point so it sticks
            currentItem.transform.parent = spawnPoint;
            currentItem.transform.localPosition = Vector3.zero; // Reset position to match the spawn point
            currentItem.transform.localRotation = Quaternion.identity; // Reset rotation

            // Mark as equipped
            if (currentItem.CompareTag("Gun"))
            {
                currentItem.GetComponent<Gun>().Equip(true);
            }
            else if (currentItem.CompareTag("Flashlight"))
            {
                ToggleFlashlight();
            }
        }
    }

    public void DropCurrentItem()
    {
        if (currentItem != null)
        {
            currentItem.SetActive(true);

            // Detach the item from the spawn point
            currentItem.transform.parent = null;
            currentItem.transform.position = spawnPoint.position;
            currentItem.transform.rotation = spawnPoint.rotation;

            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;  
                rb.useGravity = true;     
            }
            if (currentItem.CompareTag("Gun"))
            {
                currentItem.GetComponent<Gun>().Equip(false);
            }
           

            ClearItemFromUI(currentItem);

            currentItem = null;  
        }
    }

    private void ClearItemFromUI(GameObject item)
    {
   
        for (int i = 0; i < storedItems.Length; i++)
        {
            if (storedItems[i] == item)
            {
                slotTexts[i].text = "";
                slotImages[i].sprite = null;
                storedItems[i] = null;  
                break;
            }
        }
    }
}
