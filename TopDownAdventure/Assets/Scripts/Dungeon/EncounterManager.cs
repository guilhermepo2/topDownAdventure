using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class EncounterManager : MonoBehaviour {
        [Header("Random Encounter Configuration")]
        [Range(0,1)]
        public float randomEncounterChance = 0.05f;

        [Header("Random Encounter - Pokemons and IV/EV")]
        public GameObject[] wildPokemons;
        public CombatSystem.Data.Stats[] ivPool;
        public CombatSystem.Data.Stats[] evPool;

        private CombatSystem.Pokemon m_playerPokemon;

        private void Start() {
            PlayerController playerReference = FindObjectOfType<PlayerController>();
            playerReference.PlayerMoved += ProcessRandomEncounter;
            m_playerPokemon = playerReference.gameObject.GetComponentInChildren<CombatSystem.Pokemon>();
        }

        private void ProcessRandomEncounter() {
            if(Random.value < randomEncounterChance) {
                DependencyManager.Instance.TopDown.HaltTopDown();

                GameObject battlingPokemons = new GameObject("Battling Pokemons");
                GameObject playerPokemon = new GameObject("Player Pokemon");
                GameObject enemyPokemon = new GameObject("Enemy Pokemon");

                playerPokemon.transform.parent = battlingPokemons.transform;
                enemyPokemon.transform.parent = battlingPokemons.transform;

                CombatSystem.Pokemon battlingPlayerPokemon = playerPokemon.AddComponent(typeof(CombatSystem.Pokemon)) as CombatSystem.Pokemon;
                battlingPlayerPokemon.topSidePokemonSprite = m_playerPokemon.topSidePokemonSprite;
                battlingPlayerPokemon.bottomSidePokemonSprite = m_playerPokemon.bottomSidePokemonSprite;
                battlingPlayerPokemon.pokemonName = m_playerPokemon.pokemonName;
                battlingPlayerPokemon.baseStats = m_playerPokemon.baseStats;
                battlingPlayerPokemon.individualValues = m_playerPokemon.individualValues;
                battlingPlayerPokemon.effortValues = m_playerPokemon.effortValues;
                battlingPlayerPokemon.nature = m_playerPokemon.nature;
                battlingPlayerPokemon.pokemonMoves = m_playerPokemon.pokemonMoves;
                battlingPlayerPokemon.currentLevel = m_playerPokemon.currentLevel;
                battlingPlayerPokemon.currentHealth = m_playerPokemon.currentHealth;
                battlingPlayerPokemon.currentExperience = m_playerPokemon.currentExperience;
                battlingPlayerPokemon.isFainted = m_playerPokemon.isFainted;

                CombatSystem.Pokemon battlingEnemyPokemon = enemyPokemon.AddComponent(typeof(CombatSystem.Pokemon)) as CombatSystem.Pokemon;
                enemyPokemon.AddComponent(typeof(CombatSystem.AI.RandomAI));
                CombatSystem.Pokemon wildPokemon = wildPokemons.RandomOrDefault().GetComponent<CombatSystem.Pokemon>();

                battlingEnemyPokemon.topSidePokemonSprite = wildPokemon.topSidePokemonSprite;
                battlingEnemyPokemon.bottomSidePokemonSprite = wildPokemon.bottomSidePokemonSprite;
                battlingEnemyPokemon.pokemonName = wildPokemon.pokemonName;
                battlingEnemyPokemon.baseStats = wildPokemon.baseStats;
                battlingEnemyPokemon.individualValues = ivPool.RandomOrDefault();
                battlingEnemyPokemon.effortValues = evPool.RandomOrDefault();
                battlingEnemyPokemon.nature = wildPokemon.nature;
                battlingEnemyPokemon.pokemonMoves = wildPokemon.pokemonMoves;
                battlingEnemyPokemon.currentLevel = wildPokemon.currentLevel;
                battlingEnemyPokemon.currentHealth = wildPokemon.currentHealth;
                battlingEnemyPokemon.currentExperience = wildPokemon.currentExperience;
                battlingEnemyPokemon.isFainted = wildPokemon.isFainted;

                DontDestroyOnLoad(battlingPokemons);
                DependencyManager.Instance.LevelManager.LoadLevelAdditive(DependencyManager.BATTLE_SCENE);
            }
        }
    }
}
