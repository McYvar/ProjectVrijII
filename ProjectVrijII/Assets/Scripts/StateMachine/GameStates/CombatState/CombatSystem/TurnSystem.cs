using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnSystem : CombatBase {
    /// <summary>
    /// By: Yvar, Date: 5/28/2023
    /// Class that handles turnhandling for the combat phase of the game
    /// </summary>

    [SerializeField] private bool testmodus;
    [SerializeField] private ComboCounter comboCounter;
    [SerializeField] private TMP_Text gameOverText;
    private CharacterStateManager currentCharacterTurn;
    private int totalCharacters = 0;

    public Action OnHit = null;
    public Action OnReset = null;

    private int readyCharacters = 0;

    /// <summary>
    /// The win condition of the game should be when one player dies, however, for the sake of expanding we will write
    /// it out in two teams. If each member of team one is dead, then team two has won and vise vera.
    /// 
    /// So we need two teams consisting of players. In some sort of player selection menu, the start button should be available
    /// when there are at least two teams. Maybe later if there is only one player with one player selected, we can spawn in
    /// an enemy/cpu.
    /// 
    /// A dictionary is very flexible and adressing elements doesn't take up much performance, however assigning to it does,
    /// but this won't happen mid fight.
    /// 
    /// If teams ever really would become a thing, then fighting should be changed a bit to not hit teammates mid fight...
    /// Even though thats unlikeley to happen anyways...
    /// </summary>
    private Dictionary<int, CharacterStateManager[]> teams = new Dictionary<int, CharacterStateManager[]>(); // team number, members
    private int teamTurn;
    private int playerTurn;

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

    public void StartCombat() {
        readyCharacters = 0;
        playerTurn = 0;
        foreach (var team in teams) {
            foreach (var player in team.Value) {
                totalCharacters++;
            }
        }
        if (!testmodus) teamTurn = UnityEngine.Random.Range(0, totalCharacters);
        else teamTurn = 0;
        NextTurn();
    }

    private void NextTurn() {
        // to do, write a condition check if team has no healt left
        for (int i = 0; i < teams.Count; i++) {
            int dead = 0;
            foreach (var player in teams[i]) {
                if (player.GetComponent<Health>().died) dead++; // also temp solution?
            }
            if (dead == teams[i].Length) {
                EndGame(i);
                return;
            }
        }

        if (!testmodus) playerTurn++;
        if (playerTurn >= teams[teamTurn].Length && !testmodus) {
            playerTurn = 0;
            teamTurn++;
            if (teamTurn >= teams.Count) teamTurn = 0;
        }

        currentCharacterTurn = teams[teamTurn][playerTurn];
        if (!testmodus) currentCharacterTurn.SwitchState(typeof(SelectionState));
        else currentCharacterTurn.SwitchState(typeof(OnGroundMovement));

    }

    private void EndGame(int loser) {
        gameOverText.text = $"Game over! Player {loser} died!";
    }

    public void ReadyForNextTurn() {
        readyCharacters++;
        if (readyCharacters >= totalCharacters) {
            foreach(var team in teams) {
                foreach (var character in team.Value) {
                    character.SwitchState(typeof(IdleState));
                }

                readyCharacters = 0;
            }
            NextTurn();
        }
    }

    // when we press start fight, or anything such in a character selection screen, we assign the teams
    public void AddTeam(int teamNumber, CharacterStateManager[] characterList) {
        teams.Add(teamNumber, characterList);

        if (testmodus) return;
        foreach (var character in characterList) {
            OnReset += () => character.SwitchState(typeof(ResetState));
        }
    }

    public void ResetTurnSystem() {
        teams.Clear();
        totalCharacters = 0;
    }
}
