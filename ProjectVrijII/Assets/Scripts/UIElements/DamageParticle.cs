using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageParticle : MonoBehaviour {
	[SerializeField] private TMP_Text numbersText;

	[Header("Spawning")]
	[SerializeField] private Vector3 spawnPositionOffset;
	[SerializeField] private float targetDistance;
	[SerializeField, Range(0, 1f)] private float spawnAngle;
	[SerializeField, Range(0f, 1f)] private float lerpSpeed;
	[SerializeField] private float life;

	private Vector3 targetPosition;

    //CALL when getting hurt
    public void Spawn(Vector3 spawnpos, int damage) {
		numbersText.text = $"{damage}";
		transform.position = spawnpos;
		transform.localPosition += spawnPositionOffset;

		targetPosition = transform.localPosition + new Vector3(Random.Range(-spawnAngle, spawnAngle), 1) * targetDistance;
	}

    private void Update()
    {
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, lerpSpeed);
		if (Vector3.Distance(transform.localPosition, targetPosition) < life) Destroy(gameObject);
    }
}