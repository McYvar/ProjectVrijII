using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Build;

public class TurnSystem : CombatBase {
    /// <summary>
    /// By: Yvar, Date: 5/28/2023
    /// Class that handles turnhandling for the combat phase of the game
    /// </summary>

    [SerializeField] private bool testmode = false; // testmode makes sure you can only play one character, other is a dummy
    [SerializeField] private ComboCounter comboCounter;
    [SerializeField] TMP_Text currentPlayerText;
    private CharacterStateManager currentCharacterTurn;
    private CharacterStateManager[] allCharacters;
    private int turn = 0;

    public Action OnHit = null;
    public Action OnReset = null;

    private int readyCharacters = 0;

    private static TurnSystem instance;
    public static TurnSystem Instance {
        get { return instance; }
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        OnHit += comboCounter.IncreaseCombo;
        OnReset += comboCounter.EndCombo;
    }

    public override void OnUpdate() {
        base.OnUpdate();
    }

    public void InitializeCharacters(params CharacterStateManager[] characters) {
        allCharacters = characters;

        foreach (var character in allCharacters) {
            if (!testmode) OnReset += () => character.SwitchState(typeof(ResetState));
        }
    }

    public void StartCombat() {
        turn = UnityEngine.Random.Range(0, allCharacters.Length);
        NextTurn();
    }

    private void NextTurn() {
        if (!testmode) turn++;
        if (turn == allCharacters.Length) turn = 0;

        currentCharacterTurn = allCharacters[turn];
        if (testmode) currentCharacterTurn.SwitchState(typeof(OnGroundMovement));
        else currentCharacterTurn.SwitchState(typeof(SelectionState));

        currentPlayerText.text = $"Player{turn}'s turn!";
        readyCharacters = 0;
    }

    public void ReadyForNextTurn() {
        readyCharacters++;
        if (readyCharacters >= allCharacters.Length) {
            foreach (var character in allCharacters) {
                character.SwitchState(typeof(IdleState));
            }
            NextTurn();
        }
    }
}
