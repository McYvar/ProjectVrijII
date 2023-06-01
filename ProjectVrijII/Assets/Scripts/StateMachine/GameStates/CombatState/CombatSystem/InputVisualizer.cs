using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputVisualizer : CombatBase {
	[SerializeField]
	private CanvasGroup inputInfoObject;

	[Header("Movement Input")]
	[SerializeField]
	private RectTransform inputInfo;
	[SerializeField]
	private float distance = 30;
	
	[Header("Skill Input")]
	[SerializeField]
	private RectTransform[] inputKeys;
	[SerializeField]
	private Vector2 expandKeyMultiplier = new Vector2(1.3f, 1.3f);

	//These effects are protected with null checks, leaving them optional
	[Header("Optional Effects")]
	[SerializeField]
	private Image[] inputKeyGlows;
	[SerializeField]
	private bool glowOnKeyInput;
	[SerializeField]
	private float glowSpeed = 0.8f;

	private PlayerInput playerInput;
	private Vector2[] startKeyScale;
	private Coroutine specialEffectCoroutine;
	private bool[] specialEffectPlacement;

	//TODO: REMOVE call from start when player select screen is added
	private void Start() {
		AssignPlayer(FindObjectOfType<PlayerInput>().gameObject);
	}

	//CALL after player select
	public void AssignPlayer(GameObject player) {
		playerInput = player.GetComponent<PlayerInput>();

		specialEffectPlacement = new bool[inputKeys.Length];
		startKeyScale = new Vector2[inputKeys.Length];

		for(int i = 0; i < startKeyScale.Length; i++) {
			startKeyScale[i] = inputKeys[i].localScale;
			if(inputKeyGlows.Length > i) {
				if (inputKeyGlows[i]) inputKeyGlows[i].fillAmount = 0;
			}
		}
	}

	public override void OnUpdate() {
		SetInputPoint();
		SetInputKey();
	}

	//CALL to disable and re enable (is enabled by standard)
	public void EnableInputInfo(bool enable) {
		inputInfoObject.alpha = enable ? 1 : 0;
	}

	//CALL this when special skill can be used
	public void SetSpecialEffect(int keyId) {
		if(inputKeyGlows.Length > keyId) {
			if(inputKeyGlows[keyId]) {
				specialEffectPlacement[keyId] = true;

				if(specialEffectCoroutine == null) {
					specialEffectCoroutine = StartCoroutine(SpecialEffect());
				}
			}
		}
	}

	//CALL when special skill is used or no longer avaible
	public void StopSpecialEffect(int keyId) {
		if(inputKeyGlows.Length > keyId) {
			specialEffectPlacement[keyId] = false;

			if(specialEffectCoroutine != null) {
				if(!specialEffectPlacement.Contains(true)) {
					StopCoroutine(specialEffectCoroutine);
					specialEffectCoroutine = null;
				}
			}

			if(inputKeyGlows[keyId]) inputKeyGlows[keyId].fillAmount = 0;
		}
	}

	//CALL to stop all special effects at once
	public void StopSpecialEffect() {
		if(specialEffectCoroutine != null) {
			StopCoroutine(specialEffectCoroutine);
			specialEffectCoroutine = null;	
		}

		if(inputKeyGlows.Length > 0) {
			for(int i = 0; i < inputKeyGlows.Length; i++) {
				specialEffectPlacement[i] = false;
				if(inputKeyGlows[i]) inputKeyGlows[i].fillAmount = 0;
			}
		}
	}

	//Shows the movement direction
	private void SetInputPoint() {
		Vector2 newpos = Vector2.zero;
		newpos = playerInput.leftDirection;

		inputInfo.anchoredPosition = newpos * distance;
	}

	//Shows the skill key used
	private void SetInputKey() {
		ExpandKey(0, playerInput.west);
		ExpandKey(1, playerInput.south);
		ExpandKey(2, playerInput.east);
	}

	//Expands the key input size when pressed and if enabled, enable a glow on pressed
	private void ExpandKey(int keyId, bool expand) {
		if(expand) {
			inputKeys[keyId].localScale = startKeyScale[keyId] * expandKeyMultiplier;
		} else {
			inputKeys[keyId].localScale = startKeyScale[keyId];
		}

		if(glowOnKeyInput) {
			if(inputKeyGlows.Length > keyId) {
				if(inputKeyGlows[keyId]) inputKeyGlows[keyId].fillAmount = expand ? 1 : 0;
			}
		}
	}

	//A glow effect to attact attention to a certain (or multiple) skill keys
	private IEnumerator SpecialEffect() {
		bool fillUp = true;

		while(true) {
			float time = 0;
			float fillAmount = 0;

			//swapping rotation direction to keep the effect in a full circle motion,
			//otherwise it will change direction everytime it fills and unfills
			for(int i = 0; i < inputKeyGlows.Length; i++) {
				if(inputKeyGlows[i]) inputKeyGlows[i].fillClockwise = fillUp;
			}

			//toggles between slowly filling up and slowly filling down
			while((fillUp == true && fillAmount < 1) || (fillUp == false && fillAmount > 0)) {
				fillAmount = Mathf.Lerp(0, 1, time);
				
				for(int j = 0; j < specialEffectPlacement.Length; j++) {
					if(specialEffectPlacement[j]) {
						if(inputKeyGlows[j]) inputKeyGlows[j].fillAmount = fillAmount;
					}
				}

				if(fillUp) {
					time += glowSpeed * Time.deltaTime;
				} else {
					time -= glowSpeed * Time.deltaTime;
				}

				yield return new WaitForSeconds(0);
			}
			
			fillUp = !fillUp;
		}
	}
}
