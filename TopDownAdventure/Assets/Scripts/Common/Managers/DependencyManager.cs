using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyManager : MonoBehaviour {
    // Constant strings for scenes
    public static int BATTLE_SCENE = 3;

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

    private TopDownManager m_topDownManagerReference;
    public TopDownManager TopDown {
        get {
            return m_topDownManagerReference;
        }
    }

    private LevelManager m_levelManagerReference;
    public LevelManager LevelManager {
        get {
            if(m_levelManagerReference == null) {
                GameObject levelManagerObject = new GameObject("Level Manager");
                levelManagerObject.AddComponent(typeof(LevelManager));
                m_levelManagerReference = levelManagerObject.GetComponent<LevelManager>();
                levelManagerObject.transform.parent = this.transform;
            }

            return m_levelManagerReference;
        }
    }


    private Dungeon.EncounterManager m_encounterManager;


    private Dungeon.DungeonGenerator m_dungeonGenerator;

    private void Awake() {
        if (m_instance == null) {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void FetchDependencies() {
        m_topDownManagerReference = FindObjectOfType<TopDownManager>();
        m_levelManagerReference = FindObjectOfType<LevelManager>();
    }
}
