using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

/// <summary>
/// Helper script to automatically set up the GameFlow scene structure.
/// Attach this to an empty GameObject in GameFlow.unity and click "Setup Scene" in Inspector.
/// </summary>
public class GameFlowSceneSetup : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string instructions = 
        "1. Create a new scene called 'GameFlow.unity'\n" +
        "2. Add this script to an empty GameObject\n" +
        "3. Click 'Setup Scene' button below (or call SetupScene() in play mode)\n" +
        "4. Assign the created objects to respective controllers\n" +
        "5. Save the scene";

    [Header("Scene Configuration")]
    [SerializeField] private string screensSceneName = "Screens";
    [SerializeField] private string cardLibrarySceneName = "CardLibraryUI";
    [SerializeField] private string gameScreenSceneName = "GameScreen";
    [SerializeField] private string votingScreenSceneName = "VotingScreen";
    [SerializeField] private string gameBoardSceneName = "GameBoard";

    [Header("References (Auto-assigned)")]
    public GameObject gameFlowManagerObj;
    public GameObject networkManagerObj;
    public GameObject eventSystemObj;

    [ContextMenu("Setup Scene")]
    public void SetupScene()
    {
        Debug.Log("[GameFlowSceneSetup] Starting scene setup...");

        // 1. Create GameFlowManager
        gameFlowManagerObj = CreateGameFlowManager();

        // 2. Create or find NetworkManager
        networkManagerObj = SetupNetworkManager();

        // 3. Create or find EventSystem
        eventSystemObj = SetupEventSystem();

        Debug.Log("[GameFlowSceneSetup] Scene setup complete!");
        Debug.Log("[GameFlowSceneSetup] Next: Configure scene names in GameFlowManager Inspector");
    }

    private GameObject CreateGameFlowManager()
    {
        GameObject existing = GameObject.Find("GameFlowManager");
        if (existing != null)
        {
            Debug.Log("[GameFlowSceneSetup] GameFlowManager already exists");
            return existing;
        }

        GameObject obj = new GameObject("GameFlowManager");
        GameFlowManager manager = obj.AddComponent<GameFlowManager>();
        
        // Use reflection to set private serialized fields
        var type = typeof(GameFlowManager);
        var screensField = type.GetField("screensSceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var cardLibField = type.GetField("cardLibrarySceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var gameScreenField = type.GetField("gameScreenSceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var votingField = type.GetField("votingScreenSceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var boardField = type.GetField("gameBoardSceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        screensField?.SetValue(manager, screensSceneName);
        cardLibField?.SetValue(manager, cardLibrarySceneName);
        gameScreenField?.SetValue(manager, gameScreenSceneName);
        votingField?.SetValue(manager, votingScreenSceneName);
        boardField?.SetValue(manager, gameBoardSceneName);

        Debug.Log("[GameFlowSceneSetup] Created GameFlowManager");
        return obj;
    }

    private GameObject SetupNetworkManager()
    {
        NetworkManager existing = FindObjectOfType<NetworkManager>();
        if (existing != null)
        {
            Debug.Log("[GameFlowSceneSetup] NetworkManager found");
            DontDestroyOnLoad(existing.gameObject);
            return existing.gameObject;
        }

        Debug.LogWarning("[GameFlowSceneSetup] NetworkManager not found! Add it manually.");
        return null;
    }

    private GameObject SetupEventSystem()
    {
        UnityEngine.EventSystems.EventSystem existing = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (existing != null)
        {
            Debug.Log("[GameFlowSceneSetup] EventSystem found");
            return existing.gameObject;
        }

        GameObject obj = new GameObject("EventSystem");
        obj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        obj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        Debug.Log("[GameFlowSceneSetup] Created EventSystem");
        return obj;
    }
}
