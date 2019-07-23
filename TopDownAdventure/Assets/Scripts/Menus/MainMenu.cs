using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public GameObject creditsPanel;

    private void Start() {
        DependencyManager.Instance.SoundManager.PlayBackgroundMusic(DependencyManager.Instance.mainMenuMusic);
    }

    public void PlayGame() {
        DependencyManager.Instance.LevelManager.LoadNextLevel();
    }

    public void OpenCreditsPanel() {
        creditsPanel.SetActive(true);
    }

    public void CloseCreditsPanel() {
        creditsPanel.SetActive(false);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void PlayButtonClickSound() {
        DependencyManager.Instance.SoundManager.PlayEffect(DependencyManager.Instance.buttonClickClip);
    }
}
