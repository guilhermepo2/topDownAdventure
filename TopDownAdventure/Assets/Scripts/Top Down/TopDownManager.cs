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
        m_playerControllerReference.PlayerExitedBattle();
        m_playerControllerReference.enabled = true;
        m_topDownCameraReference.gameObject.SetActive(true);
    }

    public void HaltTopDown() {
        m_playerControllerReference.PlayerEnteredBattle();
        m_playerControllerReference.enabled = false;
        m_topDownCameraReference.gameObject.SetActive(false);
    }

}
