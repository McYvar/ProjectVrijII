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

	private Vector3 targetPosition;

    private void Start()
    {
		Spawn(5);
    }

    //CALL when getting hurt
    public void Spawn(int damage) {
		numbersText.text = $"{damage}";
		transform.localPosition += spawnPositionOffset;

		targetPosition = new Vector2(Random.Range(-spawnAngle, spawnAngle), 1) * targetDistance;
		Debug.Log(targetPosition);
	}

    private void Update()
    {
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, lerpSpeed);
		if (Vector3.Distance(transform.localPosition, targetPosition) < 0.1f) Destroy(gameObject);
    }
}