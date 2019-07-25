using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    [CreateAssetMenu(fileName = "Pokemon", menuName = "Combat System/Pokemon")]
    public class Pokemon : ScriptableObject {
        public enum EPokemonType {
            Normal,
            Fire,
            Fighting,
            Water,
            Flying,
            Grass,
            Poison,
            Electric,
            Ground,
            Psychic,
            Rock,
            Ice,
            Bug,
            Dragon,
            Ghost,
            Dark,
            Steel,
            Fairy
        }

        public enum EExperienceTypes {
            Fast,
            MediumFast,
            MediumSlow,
            Slow
        }

        public Sprite topSidePokemonSprite;
        public Sprite bottomSidePokemonSprite;
        public string pokemonName;
        public Data.Stats baseStats;
        public int baseExperience;
        public EExperienceTypes experienceType;
        public EPokemonType pokemonType;
    }
}
