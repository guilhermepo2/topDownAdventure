using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    [CreateAssetMenu(fileName = "Pokemon", menuName = "Combat System/Pokemon")]
    public class Pokemon : ScriptableObject {
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
    }
}
