using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltTriggerScript : MonoBehaviour
{
    public BeltSystem belt;

    private void OnTriggerEnter(Collider other)
    {
        // Only handle ammo
        if (other.CompareTag("Ammo"))
        {
            belt.PlaceAmmoOnBelt(other.gameObject);
        }
    }
}
