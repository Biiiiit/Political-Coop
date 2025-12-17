using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [Header("Scene to load (case-sensitive)")]
    [SerializeField] private string sceneName;

    public void LoadScene()
    {
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("LoadSceneButton: Scene name is empty.");
        }
    }
}
