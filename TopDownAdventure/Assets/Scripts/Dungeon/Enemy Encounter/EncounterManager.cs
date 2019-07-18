using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class EncounterManager : MonoBehaviour {

        [Header("Encounter - Pokemons and IV/EV")]
        public CombatSystem.Pokemon[] wildPokemons;
        public CombatSystem.Data.Stats[] ivPool;
        public CombatSystem.Data.Stats[] evPool;

        public void ProcessEncounter() {
            DependencyManager.Instance.TopDown.HaltTopDown();

            CombatSystem.BattlePokemon wildPokemon = DependencyManager.Instance.GetEnemyPokemon();
            wildPokemon.basePokemon = wildPokemons.RandomOrDefault();
            wildPokemon.individualValues = ivPool.RandomOrDefault();
            wildPokemon.effortValues = evPool.RandomOrDefault();

            // Do Stuff with the level Here....
            wildPokemon.currentLevel = 3;

            DependencyManager.Instance.SetEnemyPokemon(wildPokemon);
            DependencyManager.Instance.LevelManager.LoadLevelAdditive(DependencyManager.BATTLE_SCENE);
        }
    }
}
