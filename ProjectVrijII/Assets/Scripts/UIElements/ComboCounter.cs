using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class ComboCounter : CombatBase {
	[Header("CanvasElements")]
	[SerializeField]
	private TMP_Text comboText;
	[SerializeField]
	private CanvasGroup counterObject;

	[Header("CounterText")]
	[SerializeField]
	private int textSize = 20;
	/*[SerializeField]
	private string textString = "Hits!!";*/

	[Header("CounterNumbers")]
	[SerializeField]
	private TextEffect[] counterEffects;

	private int effectId = -1;
	private int comboCounter;

	[Header("OnComboFinish")]
	[SerializeField]
	private float comboDisplayTime;

	[Header("ComboTimer")]
	[SerializeField]
	private RectTransform timerFill;
	[SerializeField]
	private float emptyPosition = 92f;
	[SerializeField]
	private float fullPosition = 0;
	private Vector2 timerStartPosition;
	private Coroutine timerCoroutine;

	private float timeUntilEndingCombo = 0;

	private void Start() {		
		timerStartPosition = timerFill.localPosition;
		ResetCombo();
	}

	private void Update() {
		//~~debugging purpose
			if(Input.GetKeyUp(KeyCode.Space)) {
				IncreaseCombo(1);
			}

			if(Input.GetKeyUp(KeyCode.Escape)) {
				ResetCombo();
			}
		//~~
	}

	//Call this to reset and remove the combo
	public void ResetCombo() {
		comboCounter = 0;
		effectId = -1;
		counterObject.alpha = 0;

		ResetComboTimer();
	}

	//Call this to increase the combo and make it visible
	public void IncreaseCombo(float time) {
		comboCounter++;
		SetComboText();
		counterObject.alpha = 1;
		timeUntilEndingCombo = time;

		if(timerCoroutine == null) {
			timerCoroutine = StartCoroutine(ComboTimer());
		}
	}

	//sets the text and checks for change in effects
	private void SetComboText() {
		/*comboText.text = $"{comboCounter}<size={textSize}>{textString}</size>";*/
		comboText.text = $"{comboCounter}<size={textSize}></size>";

		if (effectId < counterEffects.Length - 1) {
			if(comboCounter >= counterEffects[effectId + 1].StartEffect) {
				effectId++;
				SetEffects();
			}
		}
	}

	private void SetEffects() {
		comboText.fontSize = counterEffects[effectId].TextSize;
		comboText.fontStyle = counterEffects[effectId].TextInBold ? FontStyles.Bold : FontStyles.Normal;
		comboText.color = counterEffects[effectId].TextColor;
		comboText.colorGradientPreset = counterEffects[effectId].Gradient;
	}

	public void EndCombo() {
		Invoke("ResetCombo", comboDisplayTime);
    }

	private IEnumerator ComboTimer() {
		float time = timeUntilEndingCombo;
		Vector2 fillerPosition = Vector2.zero;
		fillerPosition.x = timerStartPosition.x;

		while(time > 0) {
			fillerPosition.y = Mathf.Lerp(fullPosition, emptyPosition, time);
			timerFill.localPosition = fillerPosition;	
			
			time -= Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
	}

	private void ResetComboTimer() {
		if(timerCoroutine != null) {
			StopCoroutine(timerCoroutine);
			timerFill.localPosition = new Vector2(timerStartPosition.x, emptyPosition);
			timerCoroutine = null;
		}
	}
}