using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreAttackBase : CharacterBaseState {

    [SerializeField] protected GameObject menu;
    [SerializeField] protected Image[] buttons;
    private Image currentImage;
    private int currentlySelectedButton = 0;
    protected Canvas characterCanvas;

    [SerializeField] private Color unhighlightColor;
    [SerializeField] private Color highlightColor;

    protected Action[] buttonActions;

    protected override void Awake() {
        base.Awake();
        characterCanvas = GetComponentInChildren<Canvas>();
    }

    public override void OnUpdate() {
        base.OnUpdate();
    }

    public override void OnEnter() {
        base.OnEnter();
        menu.SetActive(true);
        buttonActions = new Action[buttons.Length];

        inputHandler.UpFirst += MoveUp;
        inputHandler.DownFirst += MoveDown;
        inputHandler.LeftFirst += MoveLeft;
        inputHandler.RightFirst += MoveRight;

        inputHandler.southFirst += ConfirmChoice;

        foreach (var button in buttons) {
            button.color = unhighlightColor;
        }

        currentlySelectedButton = 0;
        SwitchToButton();
    }

    public override void OnExit() {
        base.OnExit();
        menu.SetActive(false);

        inputHandler.UpFirst -= MoveUp;
        inputHandler.DownFirst -= MoveDown;
        inputHandler.LeftFirst -= MoveLeft;
        inputHandler.RightFirst -= MoveRight;

        inputHandler.southFirst -= ConfirmChoice;
    }

    private void SwitchToButton() {
        if (buttons.Length <= 0) return;
        if (currentImage != null) currentImage.color = unhighlightColor;
        currentImage = buttons[currentlySelectedButton];
        currentImage.color = highlightColor;
    }

    protected virtual void MoveUp() {
        currentlySelectedButton++;
        if (currentlySelectedButton == buttons.Length) currentlySelectedButton = 0;
        SwitchToButton();
    }
    protected virtual void MoveDown() {
        currentlySelectedButton--;
        if (currentlySelectedButton == -1) currentlySelectedButton = buttons.Length - 1;
        SwitchToButton();
    }
    protected virtual void MoveLeft() { }
    protected virtual void MoveRight() { }

    protected virtual void ConfirmChoice() {
        buttonActions[currentlySelectedButton]?.Invoke();
    }

}
