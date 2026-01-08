using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalSceneSwitchButton : MonoBehaviour
{
    [Tooltip("Exact scene name from Build Settings")]
    public string sceneName;

    public void SwitchScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty!");
            return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
