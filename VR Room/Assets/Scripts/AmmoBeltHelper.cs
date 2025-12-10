using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AmmoBeltHelper : MonoBehaviour
{
    private BeltSystem belt;

    void Start()
    {
        belt = FindObjectOfType<BeltSystem>();

        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // detach from belt if on belt
        belt.RemoveAmmoFromBelt(gameObject);
    }
}