using UnityEngine;

public class Magazine : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int replenishAmmo = 30; // Amount of ammo this magazine provides

    [Header("Tag Settings")]
    public string[] validWeaponTags; // List of valid weapon tags

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object collided with has a valid tag
        foreach (string validTag in validWeaponTags)
        {
            if (collision.gameObject.CompareTag(validTag))
            {
                // Try to get the FireBulletOnActivate script from the weapon
                FireBulletOnActivate weapon = collision.gameObject.GetComponent<FireBulletOnActivate>();
                if (weapon != null)
                {
                    weapon.AddAmmo(replenishAmmo); // Add ammo to the weapon
                    Destroy(gameObject); // Destroy the magazine after use
                }
                return; // Exit after finding a valid tag
            }
        }
    }
}