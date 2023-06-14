using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageParticle : MonoBehaviour {
	[SerializeField] private TMP_Text numbersText;

	[Header("Spawning")]
	[SerializeField] private Vector2 spawnPositionOffset;
	[SerializeField] private float spawnAngle;
	[SerializeField, Range(0f, 9f)] private float lerpSpeed;

	//CALL when getting hurt
	public void Spawn(int damage) {

	}
}