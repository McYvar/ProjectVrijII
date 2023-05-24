using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputVisualizer : CombatBase {
	[SerializeField]
	private CanvasGroup inputInfoObject;

	[SerializeField]
	private RectTransform inputInfo;

	[SerializeField]
	private float distance = 30;

	private InputHandler playerInput;

	private void Start() {
		playerInput = FindObjectOfType<InputHandler>();
	}

	public override void OnUpdate() {
		SetInputPoint();
	}

	//can be used to disable and re enable (is enabled by standard)
	public void EnableInputInfo(bool enable) {
		inputInfoObject.alpha = enable ? 1 : 0;
	}

	private void SetInputPoint() {
		Vector2 newpos = Vector2.zero;
		newpos = playerInput.leftDirection;

		inputInfo.anchoredPosition = newpos * distance;
	}
}
