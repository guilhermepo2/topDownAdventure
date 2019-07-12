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

                CombatSystem.Pokemon wildPokemon = wildPokemons.RandomOrDefault().GetComponent<CombatSystem.Pokemon>();
                wildPokemon.individualValues = ivPool.RandomOrDefault();
                wildPokemon.effortValues = evPool.RandomOrDefault();
                DependencyManager.Instance.SetEnemyPokemon(wildPokemon);

                DependencyManager.Instance.LevelManager.LoadLevelAdditive(DependencyManager.BATTLE_SCENE);
            }
        }
    }
}
