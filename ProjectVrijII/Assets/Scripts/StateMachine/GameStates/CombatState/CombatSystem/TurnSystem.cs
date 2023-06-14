using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TurnSystem : CombatBase {
    /// <summary>
    /// By: Yvar, Date: 5/28/2023
    /// Class that handles turnhandling for the combat phase of the game
    /// </summary>

    [SerializeField] private bool testmodus;
    [SerializeField] private ComboCounter comboCounter;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField, Range(0f, 60f)] private float timeToStartAttack;
    private float timer;
    private bool initialAttack = true;
    private CharacterStateManager currentCharacterTurn;
    private int totalCharacters = 0;

    [SerializeField] private TMP_Text turnTimeText;

    public Action<float> OnHit = null;
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
        OnHit += (t) =>
        {
            initialAttack = true;
            Debug.Log("hit");
        };
        OnReset += comboCounter.EndCombo;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (!initialAttack)
        {
            turnTimeText.gameObject.SetActive(true);
            turnTimeText.text = $"Time to start attack: {Math.Round(timer, 2)}";
            if (timer > 0) timer -= Time.deltaTime;
            else
            {
                OnReset.Invoke();
                initialAttack = true;
            }
        }
        else
        {
            turnTimeText.gameObject.SetActive(false);
        }
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
                if (i == 0) EndGame(1);
                if (i == 1) EndGame(2);
                return;
            }
        }

        if (!testmodus) playerTurn++;
        if (playerTurn >= teams[teamTurn].Length && !testmodus) {
            playerTurn = 0;
            teamTurn++;
            if (teamTurn >= teams.Count) teamTurn = 0;
        }

        timer = timeToStartAttack;
        readyCharacters = 0;
        currentCharacterTurn = teams[teamTurn][playerTurn];
        //if (!testmodus) currentCharacterTurn.SwitchState(typeof(SelectionState));
        StartCoroutine(DoTransition());

    }

    private void EndGame(int winner) {
        gameOverText.text = $"Game over! Player {winner} wins!";
        Invoke("ReloadSceneOnGameOver", 5);
    }

    public void ReadyForNextTurn() {
        readyCharacters++;
        if (readyCharacters >= totalCharacters) {
            foreach(var team in teams) {
                foreach (var character in team.Value) {
                    character.SwitchState(typeof(IdleState));
                }
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
        timer = timeToStartAttack;
        initialAttack = false;
    }

    public void ReloadSceneOnGameOver() {
        SceneManager.LoadScene(1);
    }

    private IEnumerator DoTransition()
    {
        yield return new WaitForSeconds(1);
        comboCounter.SetCombotTimer(timer);
        initialAttack = false;
        currentCharacterTurn.SwitchState(typeof(OnGroundMovement));
    }
}
