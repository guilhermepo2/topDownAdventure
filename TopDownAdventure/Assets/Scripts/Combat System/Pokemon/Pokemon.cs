using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    [CreateAssetMenu(fileName = "Pokemon", menuName = "Combat System/Pokemon")]
    public class Pokemon : ScriptableObject {
        public enum EPokemonType {
            Normal = 0,
            Fighting = 1,
            Flying = 2,
            Poison = 3,
            Ground = 4,
            Rock = 5,
            Bug = 6,
            Ghost = 7,
            Steel = 8,
            Fire = 9,
            Water = 10,
            Grass = 11,
            Electric = 12,
            Psychic = 13,
            Ice = 14,
            Dragon = 15,
            Dark = 16,
            Fairy = 17
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
