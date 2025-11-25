using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float Damage = 10;
	private void OnCollisionEnter(Collision collision)
	{
		EnemyAIController enemy = collision.collider.GetComponent<EnemyAIController>();

		if (enemy != null)
		{
			enemy.TakeDamage(Damage);

			if (HitMarkerManager.Instance != null)
				HitMarkerManager.Instance.ShowHitMarker();

			Destroy(gameObject);
			return;
		}

		Destroy(gameObject);

	}
}