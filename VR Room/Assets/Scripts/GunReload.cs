using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunReload : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;

    [Header("Reload Settings")]
    public string magazineTag = "Magazine"; // Tag for the magazine object

    [Header("UI")]
    public TMPro.TextMeshProUGUI ammoText;

    private void Start()
    {
        UpdateAmmoUI();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding with the gun is a magazine
        if (collision.gameObject.CompareTag(magazineTag))
        {
            ReloadWeapon();
            Destroy(collision.gameObject); // Destroy the magazine after reloading
        }
    }

    private void ReloadWeapon()
    {
        int neededAmmo = maxAmmo - currentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        UpdateAmmoUI();
        Debug.Log("Weapon reloaded!");
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
            ammoText.color = currentAmmo == 0 ? Color.red : Color.blue;
        }
    }
}