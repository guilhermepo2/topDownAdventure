﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CombatSystem {
    public class CombatManager : MonoBehaviour {
        public enum ECombatState {
            Intro,
            PlayerSelectingOption,
            PlayerSelectingMove,
            ResolveTurns,
            TurnBeingResolved,
            ProcessingTurnStack,
            EndOfTurn,
            TurnsEnded,
            PlayerWon,
            PlayerLose,
            End,
            // should I have separate states for player won and player lose?
        }

        public enum EOptionsToSelect {
            FIGHT,
            DEFEND,
            PACK,
            RUN,
        }

        public enum EMovesToSelect {
            MoveTopLeft,
            MoveTopRight,
            MoveBottomLeft,
            MoveBottomRight
        }

        [Header("Pokemons")]
        public GameObject enemyPokemon;
        public GameObject playerPokemon;
        private BattlePokemon m_enemyPokemon;
        private AI.BaseEnemyAI m_enemyAI;
        private BattlePokemon m_playerPokemon;

        [Header("UI Related")]
        public UI.PokemonUISkin enemyUISkin;
        public UI.PokemonUISkin playerUISkin;
        public TextMeshProUGUI battleLogText;
        public GameObject playerOptionsPanel;
        public GameObject movementsPanel;

        [Header("Options UI")]
        public TextMeshProUGUI fightText;
        public TextMeshProUGUI pkmnText;
        public TextMeshProUGUI packText;
        public TextMeshProUGUI runText;

        [Header("Move Selection UI UI")]
        public TextMeshProUGUI firstMove;
        public TextMeshProUGUI secondMove;
        public TextMeshProUGUI thirdMove;
        public TextMeshProUGUI fourthMove;

        private ECombatState m_combatState;

        // Turn Based Stuff
        private EOptionsToSelect m_currentlySelectedOption;
        private EMovesToSelect m_currentlySelectedMove;
        private int m_selectedMoveIndex;
        private Stack<BattlePokemon> m_turnStack;

        // Dialogue Feedback
        private Queue<string> m_feedbackSentences;

        // Constants
        private const float km_defendedMoveMultiplier = 0.35f;

        private void Start() {
            // Checking if it was transitioned from Dungeon
            Destroy(enemyPokemon);
            Destroy(playerPokemon);
            playerPokemon = DependencyManager.Instance.GetPlayerPokemon().gameObject;
            enemyPokemon = DependencyManager.Instance.GetEnemyPokemon().gameObject;

            // Setting Up UI
            m_enemyPokemon = enemyPokemon.GetComponent<BattlePokemon>();
            m_enemyAI = enemyPokemon.GetComponent<AI.BaseEnemyAI>();
            m_playerPokemon = playerPokemon.GetComponent<BattlePokemon>();
            m_enemyPokemon.CalculateStats(true);

            // [TO DO]
            // this should be false
            // using true for debugging...
            m_playerPokemon.CalculateStats(false);

            m_turnStack = new Stack<BattlePokemon>();
            m_feedbackSentences = new Queue<string>();
            enemyUISkin.Assign(m_enemyPokemon, false);
            playerUISkin.Assign(m_playerPokemon, true);
            playerOptionsPanel.SetActive(false);
            movementsPanel.SetActive(false);
            battleLogText.text = "";

            // Setting Up Combat
            m_combatState = ECombatState.Intro;
            m_currentlySelectedOption = EOptionsToSelect.FIGHT;


            ProcessBattleIntro();
        }

        private void ProcessBattleIntro() {
            battleLogText.text = $"Enemy {m_enemyPokemon.PokemonName} wants to battle!";
        }

        private void Update() {
            switch(m_combatState) {
                case ECombatState.Intro:
                    IntroState();
                    break;
                case ECombatState.PlayerSelectingOption:
                    PlayerSelectingOption();
                    break;
                case ECombatState.PlayerSelectingMove:
                    PlayerSelectingMove();
                    break;
                case ECombatState.ResolveTurns:
                    m_combatState = ECombatState.TurnBeingResolved;
                    ResolveTurns();
                    break;
                case ECombatState.TurnBeingResolved:
                    //
                    break;
                case ECombatState.ProcessingTurnStack:
                    //
                    ProcessTurnStack();
                    break;
                case ECombatState.EndOfTurn:
                    //
                    ProcessEndOfTurn();
                    break;
                case ECombatState.TurnsEnded:
                    EndTurns();
                    break;
                case ECombatState.PlayerWon:
                    PlayerWon();
                    break;
                case ECombatState.PlayerLose:
                    PlayerLost();
                    break;
                case ECombatState.End:
                    CombatEnded();
                    break;
            }
        }

        #region INTRO STATE
        private void IntroState() {
            if(Input.GetKeyDown(KeyCode.Return)) {
                battleLogText.text = "";
                playerOptionsPanel.SetActive(true);
                m_combatState = ECombatState.PlayerSelectingOption;
            }
        }
        #endregion

        #region PLAYER SELECTING OPTION
        private void PlayerSelectingOption() {
            // Handling Which Option is Selected...
            if(Input.GetKeyDown(KeyCode.W)) {
                if(m_currentlySelectedOption == EOptionsToSelect.PACK) {
                    m_currentlySelectedOption = EOptionsToSelect.FIGHT;
                } else if(m_currentlySelectedOption == EOptionsToSelect.RUN) {
                    m_currentlySelectedOption = EOptionsToSelect.DEFEND;
                }
            } else if(Input.GetKeyDown(KeyCode.S)) {
                if(m_currentlySelectedOption == EOptionsToSelect.FIGHT) {
                    m_currentlySelectedOption = EOptionsToSelect.PACK;
                } else if(m_currentlySelectedOption == EOptionsToSelect.DEFEND) {
                    m_currentlySelectedOption = EOptionsToSelect.RUN;
                }
            } else if(Input.GetKeyDown(KeyCode.D)) {
                if (m_currentlySelectedOption == EOptionsToSelect.FIGHT) {
                    m_currentlySelectedOption = EOptionsToSelect.DEFEND;
                } else if (m_currentlySelectedOption == EOptionsToSelect.PACK) {
                    m_currentlySelectedOption = EOptionsToSelect.RUN;
                }
            } else if(Input.GetKeyDown(KeyCode.A)) {
                if (m_currentlySelectedOption == EOptionsToSelect.DEFEND) {
                    m_currentlySelectedOption = EOptionsToSelect.FIGHT;
                } else if (m_currentlySelectedOption == EOptionsToSelect.RUN) {
                    m_currentlySelectedOption = EOptionsToSelect.PACK;
                }
            }

            // UI Feedback of which Options is selected
            switch(m_currentlySelectedOption) {
                case EOptionsToSelect.FIGHT:
                    fightText.text = "> FIGHT";
                    packText.text = "PACK";
                    pkmnText.text = "DEFEND";
                    runText.text = "RUN";
                    break;
                case EOptionsToSelect.PACK:
                    fightText.text = "FIGHT";
                    packText.text = "> PACK";
                    pkmnText.text = "DEFEND";
                    runText.text = "RUN";
                    break;
                case EOptionsToSelect.DEFEND:
                    fightText.text = "FIGHT";
                    packText.text = "PACK";
                    pkmnText.text = "> DEFEND";
                    runText.text = "RUN";
                    break;
                case EOptionsToSelect.RUN:
                    fightText.text = "FIGHT";
                    packText.text = "PACK";
                    pkmnText.text = "DEFEND";
                    runText.text = "> RUN";
                    break;
            }

            // If Player Press Something...
            if(Input.GetKeyDown(KeyCode.Return)) {
                switch(m_currentlySelectedOption) {
                    case EOptionsToSelect.FIGHT:
                        playerOptionsPanel.SetActive(false);
                        movementsPanel.SetActive(true);
                        m_combatState = ECombatState.PlayerSelectingMove;
                        m_currentlySelectedMove = EMovesToSelect.MoveTopLeft;
                        break;
                    case EOptionsToSelect.PACK:
                        // TO DO
                        break;
                    case EOptionsToSelect.DEFEND:
                        m_selectedMoveIndex = 5;
                        m_combatState = ECombatState.ResolveTurns;
                        playerOptionsPanel.SetActive(false);
                        break;
                    case EOptionsToSelect.RUN:
                        playerOptionsPanel.SetActive(false);
                        battleLogText.text = $"{m_playerPokemon.PokemonName} run away safely...";
                        m_combatState = ECombatState.End;
                        break;
                }
            }
        }
        #endregion PLAYER SELECTION OPTION

        #region PLAYER SELECTING MOVE
        private void PlayerSelectingMove() {

            // Setting Up Moves Text
            if (m_playerPokemon.pokemonMoves[0] != null) {
                firstMove.text = m_playerPokemon.pokemonMoves[0].moveName;
            } else {
                firstMove.text = "-";
            }

            if (m_playerPokemon.pokemonMoves[1] != null) {
                secondMove.text = m_playerPokemon.pokemonMoves[1].moveName;
            } else {
                secondMove.text = "-";
            }

            if (m_playerPokemon.pokemonMoves[2] != null) {
                thirdMove.text = m_playerPokemon.pokemonMoves[2].moveName;
            } else {
                thirdMove.text = "-";
            }

            if (m_playerPokemon.pokemonMoves[3] != null) {
                fourthMove.text = m_playerPokemon.pokemonMoves[3].moveName;
            } else {
                fourthMove.text = "-";
            }

            // Handling Movement Selection...
            if (Input.GetKeyDown(KeyCode.W)) {
                if (m_currentlySelectedMove == EMovesToSelect.MoveBottomLeft && m_playerPokemon.pokemonMoves[0] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveTopLeft;
                } else if (m_currentlySelectedMove == EMovesToSelect.MoveBottomRight && m_playerPokemon.pokemonMoves[1] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveTopRight;
                }
            } else if (Input.GetKeyDown(KeyCode.S)) {
                if (m_currentlySelectedMove == EMovesToSelect.MoveTopLeft && m_playerPokemon.pokemonMoves[2] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveBottomLeft;
                } else if (m_currentlySelectedMove == EMovesToSelect.MoveTopRight && m_playerPokemon.pokemonMoves[3] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveBottomRight;
                }
            } else if (Input.GetKeyDown(KeyCode.D)) {
                if (m_currentlySelectedMove == EMovesToSelect.MoveTopLeft && m_playerPokemon.pokemonMoves[1] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveTopRight;
                } else if (m_currentlySelectedMove == EMovesToSelect.MoveBottomLeft && m_playerPokemon.pokemonMoves[3] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveBottomRight;
                }
            } else if (Input.GetKeyDown(KeyCode.A)) {
                if (m_currentlySelectedMove == EMovesToSelect.MoveTopRight && m_playerPokemon.pokemonMoves[0] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveTopLeft;
                } else if (m_currentlySelectedMove == EMovesToSelect.MoveBottomRight && m_playerPokemon.pokemonMoves[2] != null) {
                    m_currentlySelectedMove = EMovesToSelect.MoveBottomLeft;
                }
            }

            // Adding Visual Cue on which one is selected
            switch (m_currentlySelectedMove) {
                case EMovesToSelect.MoveTopLeft:
                    firstMove.text = $"> {firstMove.text}";
                    break;
                case EMovesToSelect.MoveTopRight:
                    secondMove.text = $"> {secondMove.text}";
                    break;
                case EMovesToSelect.MoveBottomLeft:
                    thirdMove.text = $"> {thirdMove.text}";
                    break;
                case EMovesToSelect.MoveBottomRight:
                    fourthMove.text = $"> {fourthMove.text}";
                    break;
            }

            // Handling Selecting a Move
            if(Input.GetKeyDown(KeyCode.Return)) {
                switch(m_currentlySelectedMove) {
                    case EMovesToSelect.MoveTopLeft:
                        m_selectedMoveIndex = 0;
                        break;
                    case EMovesToSelect.MoveTopRight:
                        m_selectedMoveIndex = 1;
                        break;
                    case EMovesToSelect.MoveBottomLeft:
                        m_selectedMoveIndex = 2;
                        break;
                    case EMovesToSelect.MoveBottomRight:
                        m_selectedMoveIndex = 3;
                        break;
                }

                movementsPanel.SetActive(false);
                m_combatState = ECombatState.ResolveTurns;
            }
        }
        #endregion

        #region RESOLVING TURNS
        private void ResolveTurns() {
            m_turnStack.Clear();

            // Player is Defending...
            if(m_selectedMoveIndex == 5) {
                m_turnStack.Push(m_enemyPokemon);
            } else if(m_playerPokemon.BattleStats.speed >= m_enemyPokemon.BattleStats.speed) {
                m_turnStack.Push(m_enemyPokemon);
                m_turnStack.Push(m_playerPokemon);
            } else {
                m_turnStack.Push(m_playerPokemon);
                m_turnStack.Push(m_enemyPokemon);
            }

            m_combatState = ECombatState.ProcessingTurnStack;
        }
        #endregion RESOLVING TURNS

        #region PROCESSING TURN STACK
        private void ProcessTurnStack() {
            BattlePokemon pokemonTakingTurn = m_turnStack.Pop();
            BattlePokemon pokemonBeingActedOn;
            float damageMultiplier = 1.0f;

            if(pokemonTakingTurn.currentHealth <= 0) {
                battleLogText.text = $"{pokemonTakingTurn.PokemonName} fainted!";
                m_combatState = ECombatState.EndOfTurn;
                return;
            }

            // getting performed move...
            Data.Move performedMove;
            if (pokemonTakingTurn == m_playerPokemon) {
                performedMove = m_playerPokemon.pokemonMoves[m_selectedMoveIndex];
                pokemonBeingActedOn = m_enemyPokemon;
            } else {
                performedMove = m_enemyAI.SelectMovement();
                pokemonBeingActedOn = m_playerPokemon;

                if(m_selectedMoveIndex == 5) {
                    damageMultiplier = km_defendedMoveMultiplier;
                }
            }

            Debug.Log($"Debugging Purposes {CombatFunctions.damageMultiplierByType[(int)pokemonTakingTurn.basePokemon.pokemonType, (int)pokemonBeingActedOn.basePokemon.pokemonType]}");
            int damage = 0;
            int damageMultiplied = 0;

            switch (performedMove.moveCategory) {
                case Data.Move.EDamageCategory.Physical:
                    damage = CombatFunctions.CalculateDamage(pokemonTakingTurn.currentLevel, performedMove.movePower, pokemonTakingTurn.BattleStats.attack, pokemonBeingActedOn.BattleStats.defense);
                    damageMultiplied = Mathf.RoundToInt(damage * damageMultiplier);

                    if(damageMultiplied < damage) {
                        battleLogText.text = $"{pokemonTakingTurn.PokemonName} caused {damageMultiplied} damage, {pokemonBeingActedOn.PokemonName} defended!";
                    } else {
                        battleLogText.text = $"{pokemonTakingTurn.PokemonName} caused {damageMultiplied} damage!";
                    }

                    pokemonBeingActedOn.currentHealth = Mathf.Max(0, pokemonBeingActedOn.currentHealth - damageMultiplied);
                    break;
                case Data.Move.EDamageCategory.Special:
                    damage = CombatFunctions.CalculateDamage(pokemonTakingTurn.currentLevel, performedMove.movePower, pokemonTakingTurn.BattleStats.specialAttack, pokemonBeingActedOn.BattleStats.specialDefense);
                    damageMultiplied = Mathf.RoundToInt(damage * CombatFunctions.damageMultiplierByType[(int)performedMove.moveType, (int)pokemonBeingActedOn.basePokemon.pokemonType]);

                    if(damageMultiplied > damage) {
                        battleLogText.text = $"{pokemonTakingTurn.PokemonName} caused {damageMultiplied} damage! It's super effective!";
                    } else {
                        battleLogText.text = $"{pokemonTakingTurn.PokemonName} caused {damageMultiplied} damage!";
                    }

                    pokemonBeingActedOn.currentHealth = Mathf.Max(0, pokemonBeingActedOn.currentHealth - damageMultiplied);
                    break;
                case Data.Move.EDamageCategory.Status:
                    // [TO DO]
                    // How exactly to implement this?
                    // Check for every possible status movement and have code for each one?
                    if(performedMove.moveName == "LEER") {
                        pokemonBeingActedOn.BattleStats.defense = Mathf.RoundToInt((pokemonBeingActedOn.BattleStats.defense * (2.0f / 3.0f) ));
                        battleLogText.text = $"{pokemonTakingTurn.PokemonName} used LEER! {pokemonBeingActedOn.PokemonName} defense fell!";
                    }
                    break;
            }

            m_combatState = ECombatState.EndOfTurn;
        }
        #endregion PROCESSING TURN STACK

        #region PROCESSING END OF TURN
        private void ProcessEndOfTurn() {
            if(Input.GetKeyDown(KeyCode.Return)) {
                if(m_turnStack.Count > 0) {
                    m_combatState = ECombatState.ProcessingTurnStack;
                } else {
                    m_combatState = ECombatState.TurnsEnded;
                }

                enemyUISkin.Assign(m_enemyPokemon, false);
                playerUISkin.Assign(m_playerPokemon, true);
            }
        }
        #endregion

        #region END TURNS
        private void EndTurns() {
            // Check for win/loss condition
            if (m_enemyPokemon.currentHealth <= 0) {
                m_combatState = ECombatState.PlayerWon;

                // Adding sentences to be shown to the player...
                m_feedbackSentences.Clear();
                m_feedbackSentences.Enqueue($"{m_playerPokemon.PokemonName} won the battle!");
                int experienceEarned = CombatFunctions.CalculateExperience(m_enemyPokemon.basePokemon.baseExperience, m_enemyPokemon.currentLevel, m_playerPokemon.currentLevel);
                // [TO DO] add experience to player pokemon...
                m_feedbackSentences.Enqueue($"{m_playerPokemon.PokemonName} earned {experienceEarned} points of experience!");
                if(m_playerPokemon.AddExperience(experienceEarned)) {
                    m_feedbackSentences.Enqueue($"{m_playerPokemon.PokemonName} leveled up!");
                }

                battleLogText.text = m_feedbackSentences.Dequeue();
            } else if(m_playerPokemon.currentHealth <= 0) {
                m_combatState = ECombatState.PlayerLose;

                // Adding sentences to be shown to the player...
                m_feedbackSentences.Clear();
                m_feedbackSentences.Enqueue($"{m_playerPokemon.PokemonName} fainted...");
                m_feedbackSentences.Enqueue($"Game Over...");

                // Showing the first sentence...
                battleLogText.text = m_feedbackSentences.Dequeue();
            } else {
                // Everything goes on normally...
                m_combatState = ECombatState.PlayerSelectingOption;
                m_currentlySelectedOption = EOptionsToSelect.FIGHT;
                playerOptionsPanel.SetActive(true);
                battleLogText.text = "";
            }
        }
        #endregion

        #region COMBAT ENDED
        private void PlayerWon() {
            if(Input.GetKeyDown(KeyCode.Return)) {
                // have to change to end state when we have 1 sentence because CombatEnded also wait for input...
                if(m_feedbackSentences.Count == 1) {
                    // Updating the Pokemon UI to show in case leveled up...
                    playerUISkin.Assign(m_playerPokemon, true);

                    m_combatState = ECombatState.End;
                }

                battleLogText.text = m_feedbackSentences.Dequeue();
            }
        }

        private void PlayerLost() {
            if(Input.GetKeyDown(KeyCode.Return)) {
                if(m_feedbackSentences.Count == 0) {
                    DependencyManager.Instance.LevelManager.UnloadLevel(DependencyManager.BATTLE_SCENE);
                    DependencyManager.Instance.RestartGame();
                } else {
                    battleLogText.text = m_feedbackSentences.Dequeue();
                }
            }
        }

        private void CombatEnded() {
            // Persisting player's pokemon life
            DependencyManager.Instance.UpdatePlayerPokemonHealth(m_playerPokemon.currentHealth);

            if(Input.GetKeyDown(KeyCode.Return)) {
                DependencyManager.Instance.LevelManager.UnloadLevel(DependencyManager.BATTLE_SCENE);
                DependencyManager.Instance.SoundManager.PlayBackgroundMusic(DependencyManager.Instance.dungeonClip);
                DependencyManager.Instance.TopDown.ActivateTopDown();
            }
        }
        #endregion COMBAT ENDED
    }
}
