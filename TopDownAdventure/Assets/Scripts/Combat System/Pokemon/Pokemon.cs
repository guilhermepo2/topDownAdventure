using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    [CreateAssetMenu(fileName = "Pokemon", menuName = "Combat System/Pokemon")]
    public class Pokemon : ScriptableObject {
        public Sprite topSidePokemonSprite;
        public Sprite bottomSidePokemonSprite;
        public string pokemonName;
        public Data.Stats baseStats;
    }
}
