using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class BeltSlot
{
    public Transform point;
    [HideInInspector] public GameObject currentAmmo;
}

public class BeltSystem : MonoBehaviour
{
    [Header("Belt Slots")]
    public BeltSlot[] beltSlots;

    // Call this when placing ammo on the belt
    public bool PlaceAmmoOnBelt(GameObject ammo)
    {
        foreach (var slot in beltSlots)
        {
            if (slot.currentAmmo == null)
            {
                // Snap ammo to belt
                ammo.transform.SetParent(slot.point);
                ammo.transform.localPosition = Vector3.zero;
                ammo.transform.localRotation = Quaternion.identity;

                // Make kinematic so it stays on belt
                Rigidbody rb = ammo.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                // Disable XR grab while on belt
                XRGrabInteractable grab = ammo.GetComponent<XRGrabInteractable>();
                if (grab != null) grab.enabled = false;

                slot.currentAmmo = ammo;
                return true;
            }
        }
        return false; // no empty slot
    }

    // Call when removing ammo (e.g., to grab it)
    public void RemoveAmmoFromBelt(GameObject ammo)
    {
        foreach (var slot in beltSlots)
        {
            if (slot.currentAmmo == ammo)
            {
                slot.currentAmmo = null;

                // Re-enable physics and grabbing
                Rigidbody rb = ammo.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;

                XRGrabInteractable grab = ammo.GetComponent<XRGrabInteractable>();
                if (grab != null) grab.enabled = true;

                ammo.transform.SetParent(null); // detach from belt
                return;
            }
        }
    }
}