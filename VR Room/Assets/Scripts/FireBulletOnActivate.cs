using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;



public class FireBulletOnActivate : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bullet;
    public Transform spawnPoint;
    public float speed = 20f;

    [Header("XR")]
    public XRGrabInteractable grabbable;

    [Header("Ammo Settings")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip gunshotClip;
	// Start is called before the first frame update

	void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FireBullet(ActivateEventArgs args)
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();

            GameObject spawnedBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = spawnPoint.forward * speed;

            Destroy(spawnedBullet, 5f);

			if (audioSource != null && gunshotClip != null)
			{
				audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
				audioSource.PlayOneShot(gunshotClip);
			}
		}
        else
        {
            Debug.Log("Out of ammo");
        }


    }

    public void Reload()
    {
        int neededAmmo = maxAmmo - currentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        UpdateAmmoUI();
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
