using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DependencyManager : MonoBehaviour {
    // Constant strings for scenes
    public static int BATTLE_SCENE = 3;
    private const string PLAYER_POKEMON = "Player Pokemon";
    private const string ENEMY_POKEMON = "Enemy Pokemon";

    private CombatSystem.BattlePokemon m_playerPokemon;
    private CombatSystem.BattlePokemon m_enemyPokemon;

    /*
     * Managers that require some kind of configuration WILL NOT be instantiated by the Dependency Manager! 
     */

    // Dependency Manager
    private static DependencyManager m_instance;
    public static DependencyManager Instance {
        get {
            if (m_instance == null) {
                GameObject dependencyManagerObject = new GameObject("Dependency Manager");
                dependencyManagerObject.AddComponent(typeof(DependencyManager));
                m_instance = dependencyManagerObject.GetComponent<DependencyManager>();

                m_instance.FetchDependencies();
            }

            return m_instance;
        }
    }

    // Top Down Manager
    private TopDownManager m_topDownManagerReference;
    public TopDownManager TopDown {
        get {
            return m_topDownManagerReference;
        }
    }

    // Level Manager
    private LevelManager m_levelManagerReference;
    public LevelManager LevelManager {
        get {
            if (m_levelManagerReference == null) {
                GameObject levelManagerObject = new GameObject("Level Manager");
                levelManagerObject.AddComponent(typeof(LevelManager));
                m_levelManagerReference = levelManagerObject.GetComponent<LevelManager>();
                levelManagerObject.transform.parent = this.transform;
            }

            return m_levelManagerReference;
        }
    }

    // Encounter Manager
    private Dungeon.EncounterManager m_encounterManager;
    public Dungeon.EncounterManager Encounter {
        get {
            return m_encounterManager;
        }
    }

    // Dungeon Generator
    private Dungeon.DungeonGenerator m_dungeonGenerator;
    public Dungeon.DungeonGenerator Dungeon {
        get {
            return m_dungeonGenerator;
        }
    }

    // Combat Manager
    private CombatSystem.CombatManager m_combatManager;
    public CombatSystem.CombatManager Combat {
        get {
            return m_combatManager;
        }
    }

    // ----------------------------------------------------------------------------
    private void Awake() {
        if (m_instance == null) {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        Debug.Log("Dependency Manager start...");
        m_playerPokemon = GameObject.Find(PLAYER_POKEMON).GetComponent<CombatSystem.BattlePokemon>();
        m_enemyPokemon = GameObject.Find(ENEMY_POKEMON).GetComponent<CombatSystem.BattlePokemon>();
        SceneManager.sceneLoaded += LevelLoaded;
    }

    private void LevelLoaded(Scene _scene, LoadSceneMode _mode) {
        FetchDependencies();
    }

    private void FetchDependencies() {
        m_topDownManagerReference = FindObjectOfType<TopDownManager>();
        m_levelManagerReference = FindObjectOfType<LevelManager>();
        m_encounterManager = FindObjectOfType<Dungeon.EncounterManager>();
        m_dungeonGenerator = FindObjectOfType<Dungeon.DungeonGenerator>();
        m_combatManager = FindObjectOfType<CombatSystem.CombatManager>();
    }

    // ----------------------------------------------------------------------------
    // Handling Pokemons on the Dependency Manager

    public void SetEnemyPokemon(CombatSystem.BattlePokemon _pokemon) {
        m_enemyPokemon.basePokemon = _pokemon.basePokemon;
        m_enemyPokemon.individualValues = _pokemon.individualValues;
        m_enemyPokemon.effortValues = _pokemon.effortValues;
        m_enemyPokemon.nature = _pokemon.nature;
        m_enemyPokemon.pokemonMoves = _pokemon.pokemonMoves;
        m_enemyPokemon.currentLevel = _pokemon.currentLevel;
        m_enemyPokemon.currentHealth = _pokemon.currentHealth;
        m_enemyPokemon.currentExperience = _pokemon.currentExperience;
        m_enemyPokemon.isFainted = _pokemon.isFainted;
    }

    public CombatSystem.BattlePokemon GetPlayerPokemon() {
        return m_playerPokemon;
    }

    public CombatSystem.BattlePokemon GetEnemyPokemon() {
        return m_enemyPokemon;
    }
}
