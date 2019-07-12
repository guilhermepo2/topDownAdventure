using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    // Scene
    public void ReloadLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel() {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int sceneToLoad = (SceneManager.GetActiveScene().buildIndex + 1) % sceneCount;
        LoadLevel(sceneToLoad);
    }

    public void LoadLevel(int _sceneBuildIndex) {
        if (Application.CanStreamedLevelBeLoaded(_sceneBuildIndex)) {
            SceneManager.LoadScene(_sceneBuildIndex);
        } else {
            Debug.Log($"Level Manager Error: Invalid Scene Specified: Scene of Index {_sceneBuildIndex}");
        }
    }

    public void LoadLevelAdditive(int _sceneBuildIndex) {
        SceneManager.LoadScene(_sceneBuildIndex, LoadSceneMode.Additive);
    }

    public void UnloadLevel(int _sceneBuildIndex) {
        SceneManager.UnloadSceneAsync(_sceneBuildIndex);
    }
}
