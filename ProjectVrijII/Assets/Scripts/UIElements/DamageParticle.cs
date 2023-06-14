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
	[SerializeField] private Vector3 spawnPositionOffset;
	[SerializeField] private float targetDistance;
	[SerializeField, Range(0, 1f)] private float spawnAngle;
	[SerializeField, Range(0f, 1f)] private float lerpSpeed;
	[SerializeField] private float life;

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

    private void Update()
    {
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, lerpSpeed);
		if (Vector3.Distance(transform.localPosition, targetPosition) < life) Destroy(gameObject);
    }
}