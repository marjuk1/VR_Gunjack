using UnityEngine;

public class Bullet : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			Debug.Log("Hit enemy!");

			if (HitMarkerManager.Instance != null)
				HitMarkerManager.Instance.ShowHitMarker();
		}

		Destroy(gameObject); // Destroy bullet after impact
	}
}