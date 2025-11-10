using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Grenade : MonoBehaviour
{
	[Header("Explosion")]
	public GameObject explosionVfxPrefab;
	public AudioClip explosionSfx;
	public float explosionRadius = 5f;
	public float explosionForce = 700f;
	public int explosionDamage = 50;
	public float destroyAfter = 5f;

	[Header("Throw / Fuse")]
	public float minImpactSpeedToExplode = 2.0f; // speed threshold to explode on hit
	public float fuseTimeAfterRelease = 0f;      // optional fuse (0 = explode on impact)

	// internal
	Rigidbody rb;
	XRGrabInteractable grab;
	bool isHeld = false;
	bool wasThrown = false;
	Coroutine fuseCoroutine;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		grab = GetComponent<XRGrabInteractable>();
	}

	void OnEnable()
	{
		grab.selectEntered.AddListener(OnGrabbed);
		grab.selectExited.AddListener(OnReleased);
	}

	void OnDisable()
	{
		grab.selectEntered.RemoveListener(OnGrabbed);
		grab.selectExited.RemoveListener(OnReleased);
	}

	void OnGrabbed(SelectEnterEventArgs args)
	{
		isHeld = true;
		wasThrown = false;

		// If a fuse was running, cancel it while held
		if (fuseCoroutine != null)
		{
			StopCoroutine(fuseCoroutine);
			fuseCoroutine = null;
		}
	}

	void OnReleased(SelectExitEventArgs args)
	{
		isHeld = false;

		// Mark that it can explode on impact now
		wasThrown = true;

		// Optionally start fuse countdown after release
		if (fuseTimeAfterRelease > 0f)
			fuseCoroutine = StartCoroutine(FuseAndExplode(fuseTimeAfterRelease));
	}

	IEnumerator FuseAndExplode(float t)
	{
		yield return new WaitForSeconds(t);
		Explode(transform.position);
	}

	void OnCollisionEnter(Collision collision)
	{
		// If we are being held, ignore collision explosions
		if (isHeld)
			return;

		// If we were thrown (or we want to allow any dropped grenade to explode),
		// check impact speed > threshold OR explode on any collision based on setting.
		float impactSpeed = collision.relativeVelocity.magnitude;

		if (wasThrown)
		{
			if (impactSpeed >= minImpactSpeedToExplode)
			{
				Explode(collision.contacts[0].point);
			}
		}
		else
		{
			// Not thrown (dropped). Optionally explode on hit anyway if fast enough:
			if (impactSpeed >= minImpactSpeedToExplode)
				Explode(collision.contacts[0].point);
		}
	}

	void Explode(Vector3 position)
	{
		// Prevent double explosion
		if (!gameObject.activeInHierarchy) return;

		// Spawn VFX
		if (explosionVfxPrefab != null)
		{
			var vfx = Instantiate(explosionVfxPrefab, position, Quaternion.identity);
			Destroy(vfx, destroyAfter);
		}

		// Play sound
		if (explosionSfx != null)
			AudioSource.PlayClipAtPoint(explosionSfx, position);

		// Apply physics impulse to nearby rigidbodies
		Collider[] hits = Physics.OverlapSphere(position, explosionRadius);
		foreach (var col in hits)
		{
			Rigidbody hitRb = col.attachedRigidbody;
			if (hitRb != null)
			{
				hitRb.AddExplosionForce(explosionForce, position, explosionRadius, 1.0f, ForceMode.Impulse);
			}

			// Damageable interface or tag check
			

			// or use tag:
			// if (col.CompareTag("Enemy")) { ... reduce health ... }
		}

		// Destroy the grenade object (or disable)
		Destroy(gameObject);
	}

	// For debug: draw explosion radius in editor
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, explosionRadius);
	}
}