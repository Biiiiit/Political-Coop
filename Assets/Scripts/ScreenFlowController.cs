using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections;

/// <summary>
/// Controls the flow between different screens in the game.
/// Handles navigation: GameLibrary -> CardLibrary -> GameScreen -> GameBoard
/// Ensures tablets and shared screen stay synchronized.
/// </summary>
public class ScreenFlowController : NetworkBehaviour
{
    public static ScreenFlowController Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string screensScene = "Screens";
    [SerializeField] private string gameLibraryScene = "GameLibrary";
    [SerializeField] private string cardLibraryScene = "CardLibraryUI";
    [SerializeField] private string gameScreenScene = "GameScreen";

    [Header("Current State")]
    private string currentScene = "";
    private bool isTransitioning = false;

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

    private void Start()
    {
        // Track the initial scene
        currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[ScreenFlowController] Starting in scene: {currentScene}");
    }

    /// <summary>
    /// Navigate to the next screen in the flow.
    /// Called from UI buttons or game logic.
    /// </summary>
    public void NavigateToNextScreen()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("[ScreenFlowController] Already transitioning, ignoring request");
            return;
        }

        string nextScene = GetNextScene(currentScene);
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError($"[ScreenFlowController] No next scene defined for: {currentScene}");
            return;
        }

        // Check if we're networked and spawned
        if (!IsSpawned || NetworkManager == null || !NetworkManager.IsListening)
        {
            Debug.LogWarning("[ScreenFlowController] Not networked, loading scene locally");
            StartCoroutine(TransitionToSceneCoroutine(nextScene));
            return;
        }

        if (IsServer)
        {
            // Host controls scene transitions for all clients
            TransitionToSceneServerRpc(nextScene);
        }
        else
        {
            // Client requests transition
            RequestSceneTransitionServerRpc(nextScene);
        }
    }

    /// <summary>
    /// Navigate to a specific scene (for special cases like going back)
    /// </summary>
    public void NavigateToScene(string sceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("[ScreenFlowController] Already transitioning, ignoring request");
            return;
        }

        // Check if we're networked and spawned
        if (!IsSpawned || NetworkManager == null || !NetworkManager.IsListening)
        {
            Debug.LogWarning("[ScreenFlowController] Not networked, loading scene locally");
            StartCoroutine(TransitionToSceneCoroutine(sceneName));
            return;
        }

        if (IsServer)
        {
            TransitionToSceneServerRpc(sceneName);
        }
        else
        {
            RequestSceneTransitionServerRpc(sceneName);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSceneTransitionServerRpc(string sceneName)
    {
        TransitionToSceneServerRpc(sceneName);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TransitionToSceneServerRpc(string sceneName)
    {
        if (isTransitioning) return;

        Debug.Log($"[ScreenFlowController] Server transitioning to: {sceneName}");
        TransitionToSceneClientRpc(sceneName);
    }

    [ClientRpc]
    private void TransitionToSceneClientRpc(string sceneName)
    {
        StartCoroutine(TransitionToSceneCoroutine(sceneName));
    }

    private IEnumerator TransitionToSceneCoroutine(string sceneName)
    {
        isTransitioning = true;

        Debug.Log($"[ScreenFlowController] Loading scene: {sceneName}");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncLoad.isDone);

        currentScene = sceneName;
        isTransitioning = false;

        Debug.Log($"[ScreenFlowController] Scene loaded: {sceneName}");
    }

    private string GetNextScene(string current)
    {
        if (current == screensScene) return gameLibraryScene;
        if (current == gameLibraryScene) return cardLibraryScene;
        if (current == cardLibraryScene) return gameScreenScene;
        
        return null; // GameScreen is the final screen
    }

    /// <summary>
    /// Get the scene that should be shown on the shared screen for a given tablet scene
    /// </summary>
    public string GetSharedScreenSceneFor(string tabletScene)
    {
        // GameLibrary, CardLibrary -> Shared screen shows waiting/idle
        // GameScreen -> Shared screen shows board display (same scene, different canvas)
        if (tabletScene == gameScreenScene)
        {
            return gameScreenScene; // Same scene, but SharedCanvas shows the board
        }
        
        // For earlier screens, shared screen can show a waiting screen
        return null; // null means "stay on current screen" or "show waiting UI"
    }
}
