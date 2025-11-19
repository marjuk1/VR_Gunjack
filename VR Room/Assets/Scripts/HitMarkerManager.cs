using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitMarkerManager : MonoBehaviour
{
	public static HitMarkerManager Instance; // Singleton for easy access
	public Image hitMarkerImage;
	public float showDuration = 0.2f;

	private void Awake()
	{
		Instance = this;
		hitMarkerImage.enabled = false;
	}

	public void ShowHitMarker()
	{
		StartCoroutine(ShowMarkerRoutine());
	}

	private IEnumerator ShowMarkerRoutine()
	{
		hitMarkerImage.enabled = true;
		yield return new WaitForSeconds(showDuration);
		hitMarkerImage.enabled = false;
	}
}