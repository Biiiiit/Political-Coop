using UnityEngine;

/// <summary>
/// Automatically organizes GameScreen UI elements into Board and Player canvas groups.
/// Attach this to an empty GameObject in GameScreen scene.
/// </summary>
public class GameScreenUISetup : MonoBehaviour
{
    [Header("UI Elements to Find")]
    [Tooltip("Names of UI GameObjects in the scene")]
    [SerializeField] private string deckCanvasName = "DeckCanvas";
    [SerializeField] private string discussionUIName = "DiscussionUI";
    [SerializeField] private string resultsUIName = "ResultsUI";
    
    [Header("References")]
    [SerializeField] private UIModeSwitcher uiModeSwitcher;

    private void Awake()
    {
        SetupUIHierarchy();
    }

    private void SetupUIHierarchy()
    {
        // Create parent containers
        GameObject playerCanvasRoot = new GameObject("PlayerCanvasRoot");
        GameObject boardCanvasRoot = new GameObject("BoardCanvasRoot");

        // Find UI elements in scene
        GameObject deckCanvas = GameObject.Find(deckCanvasName);
        GameObject discussionUI = GameObject.Find(discussionUIName);
        GameObject resultsUI = GameObject.Find(resultsUIName);

        // Organize Player UI (Tablet view)
        if (deckCanvas != null)
        {
            deckCanvas.transform.SetParent(playerCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] Moved {deckCanvasName} to PlayerCanvasRoot");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] Could not find {deckCanvasName}");
        }

        if (discussionUI != null)
        {
            discussionUI.transform.SetParent(playerCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] Moved {discussionUIName} to PlayerCanvasRoot");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] Could not find {discussionUIName}");
        }

        // Organize Board UI (Shared screen view)
        if (resultsUI != null)
        {
            resultsUI.transform.SetParent(boardCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] Moved {resultsUIName} to BoardCanvasRoot");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] Could not find {resultsUIName}");
        }

        // Assign to UIModeSwitcher if provided
        if (uiModeSwitcher != null)
        {
            // Use reflection to set private fields
            var boardField = typeof(UIModeSwitcher).GetField("boardCanvasRoot", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var playerField = typeof(UIModeSwitcher).GetField("playerCanvasRoot", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (boardField != null)
            {
                boardField.SetValue(uiModeSwitcher, boardCanvasRoot);
                Debug.Log("[GameScreenUISetup] Assigned BoardCanvasRoot to UIModeSwitcher");
            }

            if (playerField != null)
            {
                playerField.SetValue(uiModeSwitcher, playerCanvasRoot);
                Debug.Log("[GameScreenUISetup] Assigned PlayerCanvasRoot to UIModeSwitcher");
            }
        }
        else
        {
            Debug.LogWarning("[GameScreenUISetup] UIModeSwitcher reference not set! Assign it in inspector.");
        }

        Debug.Log("[GameScreenUISetup] UI hierarchy setup complete!");
    }
}
