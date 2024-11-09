using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light flashlightLight;
    private bool isEquipped = false;

    void Start()
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = false;
        }
    }

    public void Equip(bool equip)
    {
        isEquipped = equip;
        gameObject.SetActive(equip);
    }

    public void ToggleLight()
    {
        if (isEquipped && flashlightLight != null)
        {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }
}
