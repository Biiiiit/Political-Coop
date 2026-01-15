using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [Header("Scene to load (case-sensitive)")]
    [SerializeField] private string sceneName;

    [Header("Navigation Mode")]
    [Tooltip("If true, uses ScreenFlowController.NavigateToNextScreen(). If false, loads specific scene.")]
    [SerializeField] private bool useNextSceneInFlow = false;

    public void LoadScene()
    {
        if (useNextSceneInFlow)
        {
            // Use flow controller to go to next scene in sequence
            if (ScreenFlowController.Instance != null)
            {
                ScreenFlowController.Instance.NavigateToNextScreen();
            }
            else
            {
                Debug.LogWarning("LoadSceneButton: ScreenFlowController not found, falling back to direct load");
                LoadSceneDirect();
            }
        }
        else
        {
            // Load specific scene
            if (ScreenFlowController.Instance != null)
            {
                ScreenFlowController.Instance.NavigateToScene(sceneName);
            }
            else
            {
                LoadSceneDirect();
            }
        }
    }

    private void LoadSceneDirect()
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
