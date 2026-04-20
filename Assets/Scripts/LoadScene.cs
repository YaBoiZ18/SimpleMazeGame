using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f; // Ensure time is running at normal speed when loading a new scene
        SceneManager.LoadScene(sceneName);
    }
}