using UnityEngine;

/// <summary>
/// Automatically organizes GameScreen UI elements into Board and Player canvas groups.
/// Board: Shows card being voted on (shared screen)
/// Player: Shows voting buttons and hand (tablets)
/// </summary>
public class GameScreenUISetup : MonoBehaviour
{
    [Header("Player UI (Tablets)")]
    [Tooltip("Names of UI GameObjects shown on tablets")]
    [SerializeField] private string deckCanvasName = "DeckCanvas";
    [SerializeField] private string discussionUIName = "DiscussionUI";
    
    [Header("Board UI (Shared Screen)")]
    [Tooltip("Names of UI GameObjects shown on shared screen")]
    [SerializeField] private string resultsUIName = "ResultsUI";
    // The main card anchor is typically inside DiscussionUI
    // We need to find it by searching through the hierarchy
    
    [Header("References")]
    [SerializeField] private UIModeSwitcher uiModeSwitcher;

    private void Awake()
    {
        SetupUIHierarchy();
    }

    private void SetupUIHierarchy()
    {
        Debug.Log("[GameScreenUISetup] Starting UI organization...");

        // Create parent containers
        GameObject playerCanvasRoot = new GameObject("PlayerCanvasRoot");
        GameObject boardCanvasRoot = new GameObject("BoardCanvasRoot");

        // Find UI elements in scene
        GameObject deckCanvas = GameObject.Find(deckCanvasName);
        GameObject discussionUI = GameObject.Find(discussionUIName);
        GameObject resultsUI = GameObject.Find(resultsUIName);

        // === PLAYER UI (Tablets) ===
        // Players see: Their hand + voting buttons
        
        if (deckCanvas != null)
        {
            deckCanvas.transform.SetParent(playerCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] ✅ Moved {deckCanvasName} to PlayerCanvasRoot (Tablets)");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] ⚠️ Could not find {deckCanvasName}");
        }

        if (discussionUI != null)
        {
            discussionUI.transform.SetParent(playerCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] ✅ Moved {discussionUIName} to PlayerCanvasRoot (Tablets)");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] ⚠️ Could not find {discussionUIName}");
        }

        // === BOARD UI (Shared Screen) ===
        // Shared screen shows: Current card + results
        
        if (resultsUI != null)
        {
            resultsUI.transform.SetParent(boardCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] ✅ Moved {resultsUIName} to BoardCanvasRoot (Shared Screen)");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] ⚠️ Could not find {resultsUIName}");
        }

        // Find MainCardAnchor (usually inside DiscussionUI prefab)
        // This needs to be moved to board canvas for shared screen display
        Transform mainCardAnchor = FindDeepChild(discussionUI?.transform, "MainCardAnchor");
        if (mainCardAnchor != null)
        {
            mainCardAnchor.SetParent(boardCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] ✅ Moved MainCardAnchor to BoardCanvasRoot (Shared Screen)");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] ⚠️ Could not find MainCardAnchor - looking for it in scene root...");
            GameObject mainCardObj = GameObject.Find("MainCardAnchor");
            if (mainCardObj != null)
            {
                mainCardObj.transform.SetParent(boardCanvasRoot.transform, true);
                Debug.Log($"[GameScreenUISetup] ✅ Found and moved MainCardAnchor to BoardCanvasRoot");
            }
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
                Debug.Log("[GameScreenUISetup] ✅ Assigned BoardCanvasRoot to UIModeSwitcher");
            }

            if (playerField != null)
            {
                playerField.SetValue(uiModeSwitcher, playerCanvasRoot);
                Debug.Log("[GameScreenUISetup] ✅ Assigned PlayerCanvasRoot to UIModeSwitcher");
            }
        }
        else
        {
            Debug.LogError("[GameScreenUISetup] ❌ UIModeSwitcher reference not set! Assign UIModeRoot in inspector.");
        }

        Debug.Log("[GameScreenUISetup] 🎯 UI hierarchy setup complete!");
        Debug.Log("[GameScreenUISetup] Press T = Tablet view (voting buttons + hand)");
        Debug.Log("[GameScreenUISetup] Press G = Game/Board view (card being voted on)");
    }

    /// <summary>
    /// Recursively searches for a child by name in the hierarchy
    /// </summary>
    private Transform FindDeepChild(Transform parent, string childName)
    {
        if (parent == null) return null;

        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            
            Transform found = FindDeepChild(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }
}
