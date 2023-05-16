using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class TextEffect {
	/// <summary>
	/// Made these like this cuz I didnt want to be able to change them outside the inspector.
	/// If I made them private only, I wouldnt be able to access them in another script,
	/// But a get-/setter isnt accessable in the inspector
	/// </summary>

	[Header("EffectInfo")]
	[SerializeField]
	private int startEffect;
	public int StartEffect {
		get {
			return startEffect;
		}
	}

	[Header("TextInfo")]
	[SerializeField]
	private int textSize;
	public int TextSize {
		get {
			return textSize;
		}
	}

	[SerializeField]
	private bool textInBold;
	public bool TextInBold {
		get {
			return textInBold;
		}
	}

	[Space(10)]
	[SerializeField]
	private Color textColor;
	public Color TextColor {
		get {
			return textColor;
		}
	}

	[SerializeField]
	private TMP_ColorGradient gradient;
	public TMP_ColorGradient Gradient {
		get {
			return gradient;
		}
	}
}