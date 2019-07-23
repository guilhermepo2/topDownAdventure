using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Volume Adjustments")]
    public Slider musicVolumeSlider;
    public Slider soundEffectsVolumeSlider;

    private void Start() {
        DependencyManager.Instance.SoundManager.PlayBackgroundMusic(DependencyManager.Instance.mainMenuMusic);

        musicVolumeSlider.value = DependencyManager.Instance.SoundManager.musicVolume;
        soundEffectsVolumeSlider.value = DependencyManager.Instance.SoundManager.effectsVolume;

        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void PlayGame() {
        DependencyManager.Instance.LevelManager.LoadNextLevel();
    }

    public void OpenOptionsPanel() {
        optionsPanel.SetActive(true);
    }

    public void CloseOptionsPanel() {
        optionsPanel.SetActive(false);
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

    // -------------------------------------------------
    // Music and Sound Effects Options
    // -------------------------------------------------
    public void ChangeMusicVolume(Slider _slider) {
        DependencyManager.Instance.SoundManager.UpdateMusicVolume(_slider.value);
    }

    public void ChangeSoundEffectsVolume(Slider _slider) {
        DependencyManager.Instance.SoundManager.UpdateEffectsVolume(_slider.value);
        DependencyManager.Instance.SoundManager.PlayEffect(DependencyManager.Instance.feedbackClip);
    }
}
