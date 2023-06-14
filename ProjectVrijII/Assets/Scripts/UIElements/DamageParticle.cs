using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageParticle : MonoBehaviour {
	[SerializeField]
	private CanvasGroup damageParticle;
	[SerializeField]
	private TMP_Text numbersText;

	[Header("Spawning")]
	[SerializeField]
	private Vector2 positonOffset;
	[SerializeField]
	private bool autoDisable = true;
	[SerializeField]
	private float autoDisableDelay = 1f;

	private RectTransform particleRect;
	private Coroutine visualTimerCoroutine;

	private void Start() {
		particleRect = damageParticle.GetComponent<RectTransform>();
		DisableVisual();
	}

	//CALL when getting hurt
	public void EnableVisual(Transform location, int damage) {
		Vector2 viewportPoint = Camera.main.WorldToViewportPoint(location.position);
		particleRect.localPosition = viewportPoint + positonOffset;

		numbersText.text = $"{damage}";
		damageParticle.alpha = 1;

		if(autoDisable) {
			if(visualTimerCoroutine != null) {
				StopCoroutine(visualTimerCoroutine);
				visualTimerCoroutine = null;
			}

			visualTimerCoroutine = StartCoroutine(Timer());
		}
	}

	//CALL to disable visuals
	public void DisableVisual() {
		damageParticle.alpha = 0;

		if(autoDisable) {
			if(visualTimerCoroutine != null) {
				StopCoroutine(visualTimerCoroutine);
				visualTimerCoroutine = null;
			}
		}
	}

	private IEnumerator Timer() {
		yield return new WaitForSeconds(autoDisableDelay);
		DisableVisual();
	}

}