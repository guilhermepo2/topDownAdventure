using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DependencyManager : MonoBehaviour {
    [Header("Musics")]
    public AudioClip mainMenuMusic;
    public AudioClip cityClip;
    public AudioClip dungeonClip;
    public AudioClip battleTheme;
    public AudioClip bossBattleTheme;

    [Header("Sound Effects")]
    public AudioClip feedbackClip;
    public AudioClip buttonClickClip;

    // Constant strings for scenes and configuration
    public static int MAIN_MENU_SCENE = 0;
    public static int TOWN_SCENE = 1;
    public static int DUNGEON_INTRO_SCENE = 2;
    public static int DUNGEON_SCENE = 3;
    public static int BATTLE_SCENE = 4;
    private const string PLAYER_POKEMON = "Player Pokemon";
    private const string ENEMY_POKEMON = "Enemy Pokemon";
    private const int INITIAL_LEVEL = 5;

    private CombatSystem.BattlePokemon m_playerPokemon;
    private CombatSystem.BattlePokemon m_enemyPokemon;

    // Progression Management
    private int m_currentDungeonDifficulty;
    public int DungeonDifficulty {
        get {
            return m_currentDungeonDifficulty;
        }
    }

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

    // Dungeon Controller
    private Dungeon.DungeonController m_dungeonController;
    public Dungeon.DungeonController DungeonController {
        get {
            return m_dungeonController;
        }
    }

    // Combat Manager
    private CombatSystem.CombatManager m_combatManager;
    public CombatSystem.CombatManager Combat {
        get {
            return m_combatManager;
        }
    }

    // Game Controller
    private GameController m_gameController;
    public GameController Controller {
        get {
            return m_gameController;
        }
    }

    // Sound Manager
    private SoundManager m_soundManager;
    public SoundManager SoundManager {
        get {
            return m_soundManager;
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

        // Calculating Player Pokemon Stats when the game starts because here we want to set it to max health...
        m_playerPokemon.CalculateStats(true);
        m_enemyPokemon = GameObject.Find(ENEMY_POKEMON).GetComponent<CombatSystem.BattlePokemon>();
        SceneManager.sceneLoaded += LevelLoaded;

        m_currentDungeonDifficulty = INITIAL_LEVEL;

        FetchDependencies();
    }

    private void LevelLoaded(Scene _scene, LoadSceneMode _mode) {
        FetchDependencies();

        // Playing the correct music for the scenes...
        if(_scene.buildIndex == MAIN_MENU_SCENE) {
            m_soundManager.PlayBackgroundMusic(mainMenuMusic);
        } else if(_scene.buildIndex == TOWN_SCENE) {
            m_soundManager.PlayBackgroundMusic(cityClip);
        } else if(_scene.buildIndex == DUNGEON_INTRO_SCENE) {
            m_soundManager.PlayBackgroundMusic(dungeonClip);
        } else if(_scene.buildIndex == DUNGEON_SCENE) {
            // every time the dungeon scene is loaded difficulty goes up...
            m_currentDungeonDifficulty++;

            m_soundManager.PlayBackgroundMusic(dungeonClip);
        } else if(_scene.buildIndex == BATTLE_SCENE) {
            m_soundManager.PlayBackgroundMusic(battleTheme);
        }
    }

    private void FetchDependencies() {
        m_gameController = FindObjectOfType<GameController>();
        m_levelManagerReference = FindObjectOfType<LevelManager>();
        m_topDownManagerReference = FindObjectOfType<TopDownManager>();
        m_combatManager = FindObjectOfType<CombatSystem.CombatManager>();
        m_encounterManager = FindObjectOfType<Dungeon.EncounterManager>();
        m_dungeonGenerator = FindObjectOfType<Dungeon.DungeonGenerator>();
        m_dungeonController = FindObjectOfType<Dungeon.DungeonController>();
        m_soundManager = FindObjectOfType<SoundManager>();
    }

    // ----------------------------------------------------------------------------
    // Handling Pokemons on the Dependency Manager
    // ----------------------------------------------------------------------------
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

    public void UpdatePlayerPokemonHealth(int _health) {
        m_playerPokemon.currentHealth = _health;
    }

    public CombatSystem.BattlePokemon GetEnemyPokemon() {
        return m_enemyPokemon;
    }

    // -------------------------------------------------------------------------------
    // General Game Management
    // -------------------------------------------------------------------------------
    public void RestartGame() {
        // Restarting Variables
        m_currentDungeonDifficulty = INITIAL_LEVEL;
        m_playerPokemon.currentLevel = 0;
        m_playerPokemon.currentExperience = 0;

        m_playerPokemon.CalculateStats(true);
        m_levelManagerReference.LoadLevel(MAIN_MENU_SCENE);
    }
}
