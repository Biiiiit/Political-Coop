using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections;

public class GameFlowManager : NetworkBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string screensSceneName = "Screens";
    [SerializeField] private string cardLibrarySceneName = "CardLibraryUI";
    [SerializeField] private string gameScreenSceneName = "GameScreen";
    [SerializeField] private string votingScreenSceneName = "VotingScreen";
    [SerializeField] private string gameBoardSceneName = "GameBoard";

    [Header("Current State")]
    private Phase currentPhase = Phase.Lobby;
    private bool screensLoaded = false;

    // Track which scenes are currently loaded
    private bool cardLibraryLoaded = false;
    private bool gameScreenLoaded = false;
    private bool votingScreenLoaded = false;
    private bool gameBoardLoaded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[GameFlowManager] Initialized");
    }

    private void Start()
    {
        // Load the base Screens scene
        StartCoroutine(LoadScreensScene());
    }

    private IEnumerator LoadScreensScene()
    {
        Debug.Log("[GameFlowManager] Loading Screens scene...");
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(screensSceneName, LoadSceneMode.Additive);
        
        yield return new WaitUntil(() => asyncLoad.isDone);
        
        screensLoaded = true;
        Debug.Log("[GameFlowManager] Screens scene loaded");
        
        // Set active scene for lighting/skybox purposes
        Scene screensScene = SceneManager.GetSceneByName(screensSceneName);
        if (screensScene.IsValid())
        {
            SceneManager.SetActiveScene(screensScene);
        }
        
        // Initialize with lobby phase
        OnPhaseChanged(Phase.Lobby);
    }

    // Called by BoardController or PlayerRoleController when phase changes
    public void OnPhaseChanged(Phase newPhase)
    {
        if (currentPhase == newPhase) return;
        
        Debug.Log($"[GameFlowManager] Phase changing from {currentPhase} to {newPhase}");
        
        Phase previousPhase = currentPhase;
        currentPhase = newPhase;
        
        StartCoroutine(HandlePhaseTransition(previousPhase, newPhase));
    }

    private IEnumerator HandlePhaseTransition(Phase fromPhase, Phase toPhase)
    {
        // Unload scenes from previous phase
        yield return StartCoroutine(UnloadPhaseScenesCoroutine(fromPhase));
        
        // Load scenes for new phase
        yield return StartCoroutine(LoadPhaseScenesCoroutine(toPhase));
        
        // Configure UI for new phase
        ConfigureUIForPhase(toPhase);
        
        Debug.Log($"[GameFlowManager] Phase transition complete: {toPhase}");
    }

    private IEnumerator UnloadPhaseScenesCoroutine(Phase phase)
    {
        switch (phase)
        {
            case Phase.Draw:
                if (cardLibraryLoaded)
                {
                    yield return UnloadSceneCoroutine(cardLibrarySceneName);
                    cardLibraryLoaded = false;
                }
                break;
                
            case Phase.Play:
                if (gameScreenLoaded)
                {
                    yield return UnloadSceneCoroutine(gameScreenSceneName);
                    gameScreenLoaded = false;
                }
                break;
                
            case Phase.Vote:
                if (votingScreenLoaded)
                {
                    yield return UnloadSceneCoroutine(votingScreenSceneName);
                    votingScreenLoaded = false;
                }
                if (gameBoardLoaded)
                {
                    yield return UnloadSceneCoroutine(gameBoardSceneName);
                    gameBoardLoaded = false;
                }
                break;
                
            case Phase.Resolve:
                if (gameBoardLoaded)
                {
                    yield return UnloadSceneCoroutine(gameBoardSceneName);
                    gameBoardLoaded = false;
                }
                break;
        }
    }

    private IEnumerator LoadPhaseScenesCoroutine(Phase phase)
    {
        switch (phase)
        {
            case Phase.Lobby:
                // Only Screens scene needed
                ConfigureLobbyUI();
                break;
                
            case Phase.Draw:
                // Load CardLibrary for tablets
                yield return LoadSceneCoroutine(cardLibrarySceneName);
                cardLibraryLoaded = true;
                ConfigureDrawUI();
                break;
                
            case Phase.Play:
                // Load GameScreen for tablets
                yield return LoadSceneCoroutine(gameScreenSceneName);
                gameScreenLoaded = true;
                ConfigurePlayUI();
                break;
                
            case Phase.Vote:
                // Load VotingScreen for tablets and GameBoard for shared screen
                yield return LoadSceneCoroutine(votingScreenSceneName);
                votingScreenLoaded = true;
                yield return LoadSceneCoroutine(gameBoardSceneName);
                gameBoardLoaded = true;
                ConfigureVoteUI();
                break;
                
            case Phase.Resolve:
                // Keep GameBoard loaded, ensure VotingScreen is unloaded
                if (!gameBoardLoaded)
                {
                    yield return LoadSceneCoroutine(gameBoardSceneName);
                    gameBoardLoaded = true;
                }
                if (votingScreenLoaded)
                {
                    yield return UnloadSceneCoroutine(votingScreenSceneName);
                    votingScreenLoaded = false;
                }
                ConfigureResolveUI();
                break;
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        Debug.Log($"[GameFlowManager] Loading scene: {sceneName}");
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        yield return new WaitUntil(() => asyncLoad.isDone);
        
        Debug.Log($"[GameFlowManager] Scene loaded: {sceneName}");
    }

    private IEnumerator UnloadSceneCoroutine(string sceneName)
    {
        Debug.Log($"[GameFlowManager] Unloading scene: {sceneName}");
        
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogWarning($"[GameFlowManager] Scene {sceneName} not loaded, skipping unload");
            yield break;
        }
        
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        
        yield return new WaitUntil(() => asyncUnload.isDone);
        
        Debug.Log($"[GameFlowManager] Scene unloaded: {sceneName}");
    }

    // ===== UI Configuration Methods =====

    private void ConfigureUIForPhase(Phase phase)
    {
        // This method is called after scenes are loaded
        // It configures the UI elements in Screens.unity based on phase
        
        if (BoardUIController.LocalInstance != null)
        {
            ConfigureBoardUIForPhase(phase);
        }
        
        if (PlayerUIController.LocalInstance != null)
        {
            ConfigurePlayerUIForPhase(phase);
        }
    }

    private void ConfigureLobbyUI()
    {
        Debug.Log("[GameFlowManager] Configuring Lobby UI");
    }

    private void ConfigureDrawUI()
    {
        Debug.Log("[GameFlowManager] Configuring Draw UI");
    }

    private void ConfigurePlayUI()
    {
        Debug.Log("[GameFlowManager] Configuring Play UI");
    }

    private void ConfigureVoteUI()
    {
        Debug.Log("[GameFlowManager] Configuring Vote UI");
    }

    private void ConfigureResolveUI()
    {
        Debug.Log("[GameFlowManager] Configuring Resolve UI");
    }

    private void ConfigureBoardUIForPhase(Phase phase)
    {
        var boardUI = BoardUIController.LocalInstance;
        
        switch (phase)
        {
            case Phase.Lobby:
                boardUI.ShowWaitingScreen("Waiting for players to join...");
                boardUI.EnableNextPhase(true);
                break;
                
            case Phase.Draw:
                boardUI.ShowWaitingScreen("Players are selecting cards from the library...");
                boardUI.EnableNextPhase(true);
                break;
                
            case Phase.Play:
                boardUI.ShowWaitingScreen("Players are playing their cards...");
                boardUI.EnableNextPhase(false); // Auto-advances
                break;
                
            case Phase.Vote:
                boardUI.HideWaitingScreen();
                boardUI.EnableNextPhase(false); // Auto-advances
                // GameBoard scene handles the display
                break;
                
            case Phase.Resolve:
                boardUI.HideWaitingScreen();
                boardUI.EnableNextPhase(true);
                // GameBoard scene shows results
                break;
        }
    }

    private void ConfigurePlayerUIForPhase(Phase phase)
    {
        var playerUI = PlayerUIController.LocalInstance;
        
        switch (phase)
        {
            case Phase.Lobby:
                playerUI.ShowInfo("Waiting for game to start...");
                playerUI.HideAllPhaseUI();
                break;
                
            case Phase.Draw:
                playerUI.HideAllPhaseUI();
                // CardLibrary scene handles the UI
                break;
                
            case Phase.Play:
                playerUI.HideAllPhaseUI();
                // GameScreen scene handles the UI
                break;
                
            case Phase.Vote:
                playerUI.HideAllPhaseUI();
                playerUI.ShowVoteButtons(true);
                break;
                
            case Phase.Resolve:
                playerUI.HideAllPhaseUI();
                playerUI.ShowResults();
                break;
        }
    }

    // ===== Integration with GameManager =====

    public void OnNextPhaseButtonClicked()
    {
        Debug.Log("[GameFlowManager] Next phase button clicked");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextPhaseServerRpc();
        }
    }

    public void OnPlayerPlayCard(string cardId)
    {
        Debug.Log($"[GameFlowManager] Player playing card: {cardId}");
        
        if (GameManager.Instance != null)
        {
            ulong clientId = NetworkManager.Singleton.LocalClientId;
            GameManager.Instance.PlayCardServerRpc(clientId, cardId);
        }
    }

    public void OnPlayerVote(string cardId, bool voteYes)
    {
        Debug.Log($"[GameFlowManager] Player voting {(voteYes ? "YES" : "NO")} on card: {cardId}");
        
        if (GameManager.Instance != null)
        {
            ulong clientId = NetworkManager.Singleton.LocalClientId;
            GameManager.Instance.VoteOnCardServerRpc(clientId, cardId, voteYes);
        }
    }

    // ===== Public Getters =====

    public Phase CurrentPhase => currentPhase;
    public bool IsSceneLoaded(string sceneName) => SceneManager.GetSceneByName(sceneName).isLoaded;
}
