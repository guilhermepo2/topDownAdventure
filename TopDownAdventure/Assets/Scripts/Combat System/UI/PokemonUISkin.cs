using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CombatSystem.UI {
    public class PokemonUISkin : MonoBehaviour {
        [Header("UI Elements")]
        public Image pokemonSprite;
        public TextMeshProUGUI pokemonName;
        public TextMeshProUGUI pokemonLevel;
        public TextMeshProUGUI pokemonHp;
        public TextMeshProUGUI pokemonExp;

        public void Assign(Pokemon _pokemon, bool _isBottom) {
            if(_isBottom) {
                pokemonSprite.sprite = _pokemon.bottomSidePokemonSprite;
            } else {
                pokemonSprite.sprite = _pokemon.topSidePokemonSprite;
            }

            this.pokemonName.text = _pokemon.pokemonName;
            this.pokemonLevel.text = $"L{_pokemon.currentLevel}";
            this.pokemonHp.text = $"HP {_pokemon.currentHealth}/{_pokemon.BattleStats.maxHp}";
            this.pokemonExp.text = $"EXP {_pokemon.currentExperience}";
        }
    }
}
