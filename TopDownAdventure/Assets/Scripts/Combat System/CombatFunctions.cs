using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    public static class CombatFunctions {
        public static float[,] damageMultiplierByType = new float[,]
        {
            // Normal
            { 1, 1, 1, 1, 1, 0.5f, 1, 0, 0.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            // Fight
            { 2, 1, .5f, .5f, 1, 2, .5f, 0, 2, 1, 1, 1, 1, .5f, 2, 1, 2, .5f },
            // Flying
            { 1, 2, 1, 1, 1, .5f, 2, 1, .5f, 1, 1, 2, .5f, 1, 1, 1, 1, 1 },
            // Poison
            { 1, 1, 1, .5f, .5f, .5f, 1, .5f, 0, 1, 1, 2, 1, 1, 1, 1, 1, 2 },
            // Ground
            { 1, 1, 0, 2, 1, 2, .5f, 1, 2, 2, 1, .5f, 2, 1, 1, 1, 1, 1 },
            // Rock
            { 1, .5f, 2, 1, .5f, 1, 2, 1, .5f, 2, 1, 1, 1, 1, 2, 1, 1, 1 },
            // Bug
            { 1, .5f, .5f, .5f, 1, 1, 1, .5f, .5f, .5f, 1, 2, 1, 2, 1, 1, 2, .5f },
            // Ghost
            { 0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, .5f, 1 },
            // Steel
            { 1, 1, 1, 1, 1, 2, 1, 1, .5f, .5f, .5f, 1, .5f, 1, 2, 1, 1, 2 },
            // Fire
            { 1, 1, 1, 1, 1, .5f, 2, 1, 2, .5f, .5f, 2, 1, 1, 2, .5f, 1, 1, },
            // Water
            { 1, 1, 1, 1, 2, 2, 1, 1, 1, 2, .5f, .5f, 1, 1, 1, .5f, 1, 1 },
            // Grass
            { 1, 1, .5f, .5f, 2, 2, .5f, 1, .5f, .5f, 2, .5f, 1, 1, 1, .5f, 1, 1 },
            // Electric
            { 1, 1, 2, 1, 0, 1, 1, 1, 1, 1, 2, .5f, .5f, 1, 1, .5f, 1, 1 },
            // Psychic
            { 1, 2, 1, 2, 1, 1, 1, 1, .5f, 1, 1, 1, 1, .5f, 1, 1, 0, 1 },
            // Ice
            { 1, 1, 2, 1, 2, 1, 1, 1, .5f, .5f, .5f, 2, 1, 1, .5f, 2, 1, 1 },
            // Dragon
            { 1, 1, 1, 1, 1, 1, 1, 1, .5f, 1, 1, 1, 1, 1, 1, 2, 1, 0 },
            // Dark
            { 1, .5f, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, .5f, .5f },
            // Fairy
            { 1, 2, 1, .5f, 1, 1, 1, 1, .5f, .5f, 1, 1, 1, 1, 1, 2, 2, 1 }
        };

        public static Data.InBattleStats CalculateInBattleStats(Data.Stats _baseStats, Data.Stats _iv, Data.Stats _ev, int _level) {
            Data.InBattleStats pokemonInBattleStats = new Data.InBattleStats();

            pokemonInBattleStats.maxHp = CalculateHP(_baseStats.hp, _iv.hp, _ev.hp, _level);
            pokemonInBattleStats.attack = CalculateSingleStat(_baseStats.attack, _iv.attack, _ev.attack, _level);
            pokemonInBattleStats.defense = CalculateSingleStat(_baseStats.defense, _iv.defense, _ev.defense, _level);
            pokemonInBattleStats.specialAttack = CalculateSingleStat(_baseStats.specialAttack, _iv.specialAttack, _ev.specialAttack, _level);
            pokemonInBattleStats.specialDefense = CalculateSingleStat(_baseStats.specialDefense, _iv.specialDefense, _ev.specialDefense, _level);
            pokemonInBattleStats.speed = CalculateSingleStat(_baseStats.speed, _iv.speed, _ev.speed, _level);
            pokemonInBattleStats.evasion = 100;
            pokemonInBattleStats.accuracy = 100;

            return pokemonInBattleStats;
        }

        public static int CalculateHP(int _baseHP, int _ivHP, int _evHP, int _level) {
            return Mathf.FloorToInt((((2 * _baseHP + _ivHP + (_evHP / 4)) * _level) / 100) + _level + 10);
        }

        public static int CalculateSingleStat(int _baseValue, int _ivValue, int _evValue, int _level) {
            return Mathf.FloorToInt( (((2 * _baseValue + _ivValue + (_evValue / 4)) * _level) / 100) + 5 );
        }

        public static int CalculateDamage(int _attackingPokemonLevel, int _movePower, int _attack, int _defense) {
            // TO DO
            // ADD MODIFIERS
            return (((((2 * _attackingPokemonLevel) / 5) + 2) * _movePower * (_attack / _defense)) / 50) + 2;
        }

        public static int CalculateExperience(int _baseExperience, int _defeatedPokemonLevel, int _winnerPokemonLevel) {
            float factor1 = (_baseExperience * _defeatedPokemonLevel / 5);
            float factor2_up = Mathf.Pow((2 * _defeatedPokemonLevel + 10), 2.5f);
            float factor2_down = Mathf.Pow((_defeatedPokemonLevel + _winnerPokemonLevel + 10), 2.5f);
            int totalExperience = Mathf.RoundToInt(factor1 * (factor2_up / factor2_down) + 1);
            return totalExperience;
        }

        public static int ExperienceToNextLevel(Pokemon.EExperienceTypes _experienceType, int _currentLevel) {
            switch (_experienceType) {
                case Pokemon.EExperienceTypes.Fast:
                    return CalculateFastExperienceToNextLevel(_currentLevel);
                case Pokemon.EExperienceTypes.MediumFast:
                    return CalculateMediumFastExperienceToNextLevel(_currentLevel);
                case Pokemon.EExperienceTypes.MediumSlow:
                    return CalculateMediumSlowExperienceToNextLevel(_currentLevel);
                case Pokemon.EExperienceTypes.Slow:
                    return CalculateSlowExperienceToNextLevel(_currentLevel);
            }

            return 0;
        }

        public static int CalculateFastExperienceToNextLevel(int _currentLevel) {
            int experience = Mathf.RoundToInt( (4 * Mathf.Pow(_currentLevel,3)) / (5.0f));
            return experience;
        }

        public static int CalculateMediumFastExperienceToNextLevel(int _currentLevel) {
            int experience = Mathf.RoundToInt(Mathf.Pow(_currentLevel, 3));
            return experience;
        }

        public static int CalculateMediumSlowExperienceToNextLevel(int _currentLevel) {
            int experience = Mathf.RoundToInt((6/5 * Mathf.Pow(_currentLevel, 3)) - (15 * Mathf.Pow(_currentLevel, 2)) + (100 * _currentLevel) - 140);
            return experience;
        }

        public static int CalculateSlowExperienceToNextLevel(int _currentLevel) {
            int experience = Mathf.RoundToInt( (5 * Mathf.Pow(_currentLevel, 3)) / (4.0f) );
            return experience;
        }
    }
}
