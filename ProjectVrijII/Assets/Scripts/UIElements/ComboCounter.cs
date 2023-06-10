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

	private float timeUntilEndingCombo = 0;

	private void Start() {
		ResetCombo();
	}

	//Call this to reset and remove the combo
	public void ResetCombo() {
		comboCounter = 0;
		effectId = -1;
		counterObject.alpha = 0;
	}

	//Call this to increase the combo and make it visible
	public void IncreaseCombo(float time) {
		comboCounter++;
		SetComboText();
		counterObject.alpha = 1;
		timeUntilEndingCombo = time;
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
}