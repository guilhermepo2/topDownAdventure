using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject pausePanel;

    private bool m_isGameCurrentlyPaused;
    private PlayerController m_playerControllerReference;

    private void Start() {
        m_isGameCurrentlyPaused = false;
        pausePanel.SetActive(false);
        m_playerControllerReference = FindObjectOfType<PlayerController>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    private void TogglePause() {
        ProcessPause(!m_isGameCurrentlyPaused);
    }

    public void ProcessPause(bool _pauseStatus) {
        m_isGameCurrentlyPaused = _pauseStatus;
        pausePanel.SetActive(_pauseStatus);
        Time.timeScale = _pauseStatus ? 0 : 1;
        m_playerControllerReference.enabled = !_pauseStatus;
    }

    public void ReturnToMainMenu() {
        Time.timeScale = 1;

        DependencyManager.Instance.LevelManager.LoadLevel(DependencyManager.MAIN_MENU_SCENE);
    }
}
