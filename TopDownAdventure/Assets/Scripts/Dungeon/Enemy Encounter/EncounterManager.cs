using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class EncounterManager : MonoBehaviour {

        [Header("Encounter - Pokemons and IV/EV")]
        public CombatSystem.Pokemon[] wildPokemons;
        public CombatSystem.Data.Stats[] ivPool;
        public CombatSystem.Data.Stats[] evPool;

        [Header("Wild Encounter Configuration")]
        [Range(0,1)]
        public float minimumLevelMultiplier = 0.5f;
        [Range(0, 1)]
        public float maximumLevelMultiplier = 0.7f;

        public void ProcessEncounter() {
            DependencyManager.Instance.TopDown.HaltTopDown();

            CombatSystem.BattlePokemon wildPokemon = DependencyManager.Instance.GetEnemyPokemon();
            wildPokemon.basePokemon = wildPokemons.RandomOrDefault();
            wildPokemon.individualValues = ivPool.RandomOrDefault();
            wildPokemon.effortValues = evPool.RandomOrDefault();

            int currentDifficultyLevel = DependencyManager.Instance.DungeonDifficulty;
            int minimumEncounterLevel = Mathf.FloorToInt(currentDifficultyLevel * minimumLevelMultiplier);
            int maximumEncounterLevel = Mathf.FloorToInt(currentDifficultyLevel * maximumLevelMultiplier);

            // when dealing with ints Random.Range returns [min, max)
            wildPokemon.currentLevel = Random.Range(minimumEncounterLevel, maximumEncounterLevel + 1);

            DependencyManager.Instance.SetEnemyPokemon(wildPokemon);
            DependencyManager.Instance.LevelManager.LoadLevelAdditive(DependencyManager.BATTLE_SCENE);
        }
    }
}
