using UnityEngine;
using UnityEngine.SceneManagement;
public enum SceneType
{
    Title,
    Loading,
    Lobby,
    InGame,
}

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    public void LoadScene(SceneType sceneType)
    {
        Logger.Log($"Loading scene: {sceneType}", this);

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneType.ToString());
    }

    public void ReloadScene()
    {
        Logger.Log($"Reloading scene", this);


        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
