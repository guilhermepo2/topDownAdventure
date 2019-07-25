using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TopDownManager : MonoBehaviour {
    public TilemapRenderer collisionObject;

    private PlayerController m_playerControllerReference;
    private Camera m_topDownCameraReference;

    private void Start() {
        if(collisionObject) {
            collisionObject.enabled = false;
        }

        m_playerControllerReference = FindObjectOfType<PlayerController>();
        m_topDownCameraReference = Camera.main;
    }

    public void ActivateTopDown() {
        // Activating all enemies
        Dungeon.EnemyEncounter[] allEnemies = FindObjectsOfType<Dungeon.EnemyEncounter>();
        foreach (Dungeon.EnemyEncounter enemy in allEnemies) {
            enemy.enabled = true;
        }

        m_playerControllerReference.PlayerExitedBattle();
        m_playerControllerReference.enabled = true;
        m_topDownCameraReference.gameObject.SetActive(true);
    }

    public void HaltTopDown() {
        // Deactivating all enemies
        Dungeon.EnemyEncounter[] allEnemies = FindObjectsOfType<Dungeon.EnemyEncounter>();
        foreach(Dungeon.EnemyEncounter enemy in allEnemies) {
            enemy.enabled = false;
        }

        m_playerControllerReference.PlayerEnteredBattle();
        m_playerControllerReference.enabled = false;
        m_topDownCameraReference.gameObject.SetActive(false);
    }

}
