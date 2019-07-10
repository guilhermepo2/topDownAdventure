using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CombatSystem {
    public class CombatManager : MonoBehaviour {
        public enum ECombatState {
            Intro,
            PlayerSelectingOption,
            PlayerSelectingMove,
            PlayerTurn,
            EnemyTurn,
            End,
        }

        public enum EOptionsToSelect {
            FIGHT,
            PKMN,
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
        private Pokemon m_enemyPokemon;
        private Pokemon m_playerPokemon;

        [Header("UI Related")]
        public UI.PokemonUISkin enemyUISkin;
        public UI.PokemonUISkin playerUISkin;
        public TextMeshProUGUI battleLogText;
        public GameObject playerOptionsPanel;

        [Header("Options UI")]
        public TextMeshProUGUI fightText;
        public TextMeshProUGUI pkmnText;
        public TextMeshProUGUI packText;
        public TextMeshProUGUI runText;

        private ECombatState m_combatState;

        // Turn Based Stuff
        private bool m_isPlayerTurn;
        private EOptionsToSelect m_currentlySelectedOption;

        private void Start() {
            // Setting Up UI
            m_enemyPokemon = enemyPokemon.GetComponent<Pokemon>();
            m_playerPokemon = playerPokemon.GetComponent<Pokemon>();
            enemyUISkin.Assign(m_enemyPokemon);
            playerUISkin.Assign(m_playerPokemon);
            playerOptionsPanel.SetActive(false);
            battleLogText.text = "";

            // Setting Up Combat
            m_combatState = ECombatState.Intro;
            m_currentlySelectedOption = EOptionsToSelect.FIGHT;
            m_isPlayerTurn = m_playerPokemon.BattleStats.speed >= m_enemyPokemon.BattleStats.speed;

            ProcessBattleIntro();
        }

        private void ProcessBattleIntro() {
            battleLogText.text = $"Enemy {m_enemyPokemon.pokemonName} wants to battle!";
        }

        private void Update() {
            switch(m_combatState) {
                case ECombatState.Intro:
                    IntroState();
                    break;
                case ECombatState.PlayerSelectingOption:
                    PlayerSelectingOption();
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
                    m_currentlySelectedOption = EOptionsToSelect.PKMN;
                }
            } else if(Input.GetKeyDown(KeyCode.S)) {
                if(m_currentlySelectedOption == EOptionsToSelect.FIGHT) {
                    m_currentlySelectedOption = EOptionsToSelect.PACK;
                } else if(m_currentlySelectedOption == EOptionsToSelect.PKMN) {
                    m_currentlySelectedOption = EOptionsToSelect.RUN;
                }
            } else if(Input.GetKeyDown(KeyCode.D)) {
                if (m_currentlySelectedOption == EOptionsToSelect.FIGHT) {
                    m_currentlySelectedOption = EOptionsToSelect.PKMN;
                } else if (m_currentlySelectedOption == EOptionsToSelect.PACK) {
                    m_currentlySelectedOption = EOptionsToSelect.RUN;
                }
            } else if(Input.GetKeyDown(KeyCode.A)) {
                if (m_currentlySelectedOption == EOptionsToSelect.PKMN) {
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
                    pkmnText.text = "PKMN";
                    runText.text = "RUN";
                    break;
                case EOptionsToSelect.PACK:
                    fightText.text = "FIGHT";
                    packText.text = "> PACK";
                    pkmnText.text = "PKMN";
                    runText.text = "RUN";
                    break;
                case EOptionsToSelect.PKMN:
                    fightText.text = "FIGHT";
                    packText.text = "PACK";
                    pkmnText.text = "> PKMN";
                    runText.text = "RUN";
                    break;
                case EOptionsToSelect.RUN:
                    fightText.text = "FIGHT";
                    packText.text = "PACK";
                    pkmnText.text = "PKMN";
                    runText.text = "> RUN";
                    break;
            }

            // If Player Press Something...
            if(Input.GetKeyDown(KeyCode.Return)) {
                switch(m_currentlySelectedOption) {
                    case EOptionsToSelect.FIGHT:
                        Debug.Log("Player Selected Fight!");
                        break;
                    case EOptionsToSelect.PACK:
                        // TO DO
                        break;
                    case EOptionsToSelect.PKMN:
                        // TO DO
                        break;
                    case EOptionsToSelect.RUN:
                        Debug.Log("Player Selected RUN");
                        break;
                }
            }
        }
        #endregion PLAYER SELECTION OPTION
    }
}
