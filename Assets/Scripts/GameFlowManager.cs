using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Loop order (scene names must match Build Settings exactly)")]
    [SerializeField] private string[] gameplayScenes = new[]
    {
        "GameScreen",
        "GameBoard"
    };

    [SerializeField] private bool loop = true;

    private int index = 0;
    private bool running = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGameplayFlow()
    {
        if (gameplayScenes == null || gameplayScenes.Length == 0)
        {
            Debug.LogError("[GameFlowManager] gameplayScenes is empty.");
            return;
        }

        running = true;
        index = 0;
        LoadCurrent();
    }

    public void GoNext()
    {
        if (!running)
        {
            Debug.LogWarning("[GameFlowManager] GoNext called but flow is not running.");
            return;
        }

        Debug.Log("[GameFlowManager] GoNext() called.");

        index++;

        if (index >= gameplayScenes.Length)
        {
            if (!loop)
            {
                running = false;
                Debug.Log("[GameFlowManager] Flow finished (loop disabled).");
                return;
            }

            index = 0; // loop
        }

        LoadCurrent();
    }

    private void LoadCurrent()
    {
        string scene = gameplayScenes[index];
        Debug.Log("[GameFlowManager] Loading: " + scene);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
