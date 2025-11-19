using UnityEngine;

public class Bullet : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Untagged"))
		{
			Debug.Log("Hit enemy!");

			if (HitMarkerManager.Instance != null)
				HitMarkerManager.Instance.ShowHitMarker();
		}

	}
}