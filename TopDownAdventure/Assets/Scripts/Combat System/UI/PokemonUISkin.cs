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

        // [TO DO]
        // Bool parameter is bad!!
        public void Assign(BattlePokemon _pokemon, bool _isBottom) {
            if(_isBottom) {
                pokemonSprite.sprite = _pokemon.basePokemon.bottomSidePokemonSprite;
                this.pokemonExp.text = $"EXP {_pokemon.currentExperience}/{CombatFunctions.ExperienceToNextLevel(_pokemon.basePokemon.experienceType, _pokemon.currentLevel)}";
            } else {
                pokemonSprite.sprite = _pokemon.basePokemon.topSidePokemonSprite;
                this.pokemonExp.text = "";
            }

            this.pokemonName.text = _pokemon.PokemonName;
            this.pokemonLevel.text = $"Lv. {_pokemon.currentLevel}";
            this.pokemonHp.text = $"HP {_pokemon.currentHealth}/{_pokemon.BattleStats.maxHp}";
        }
    }
}
