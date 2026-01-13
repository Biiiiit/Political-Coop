using UnityEngine;

/// <summary>
/// Splits DiscussionUI into separate Board and Player components.
/// Board: Shows MainCardAnchor (card being voted on)
/// Player: Shows voting buttons (Yes/No)
/// </summary>
public class GameScreenUISetup : MonoBehaviour
{
    [Header("GameObject Names to Find")]
    [SerializeField] private string deckCanvasName = "DeckCanvas";
    [SerializeField] private string discussionUIName = "DiscussionUI";
    [SerializeField] private string resultsUIName = "ResultsUI";
    
    [Header("Children to Split from DiscussionUI")]
    [Tooltip("These will go to BOARD (shared screen)")]
    [SerializeField] private string[] boardChildren = { "MainCardAnchor" };
    
    [Tooltip("These will stay in PLAYER (tablets) - leave empty to keep all remaining")]
    [SerializeField] private string[] playerChildren = { "VoteYesButton", "VoteNoButton", "TimerBar", "VotingButtons" };
    
    [Header("References")]
    [SerializeField] private UIModeSwitcher uiModeSwitcher;

    private void Awake()
    {
        SetupUIHierarchy();
    }

    private void SetupUIHierarchy()
    {
        Debug.Log("[GameScreenUISetup] ============================================");
        Debug.Log("[GameScreenUISetup] Starting UI Split...");

        // Create parent containers
        GameObject playerCanvasRoot = new GameObject("PlayerCanvasRoot");
        GameObject boardCanvasRoot = new GameObject("BoardCanvasRoot");

        // Find main UI elements
        GameObject deckCanvas = GameObject.Find(deckCanvasName);
        GameObject discussionUI = GameObject.Find(discussionUIName);
        GameObject resultsUI = GameObject.Find(resultsUIName);

        // === PLAYER CANVAS (Tablets) ===
        // Move entire DeckCanvas (player's hand)
        if (deckCanvas != null)
        {
            deckCanvas.transform.SetParent(playerCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] ✅ PLAYER: Moved {deckCanvasName}");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] ⚠️ Could not find {deckCanvasName}");
        }

        // === SPLIT DiscussionUI ===
        if (discussionUI != null)
        {
            Debug.Log($"[GameScreenUISetup] 🔪 SPLITTING {discussionUIName}...");
            
            // Create a container for player-side voting UI
            GameObject votingUIContainer = new GameObject("VotingUI");
            votingUIContainer.transform.SetParent(playerCanvasRoot.transform, false);
            
            // Copy canvas properties if DiscussionUI is a canvas
            Canvas discussionCanvas = discussionUI.GetComponent<Canvas>();
            if (discussionCanvas != null)
            {
                Canvas votingCanvas = votingUIContainer.AddComponent<Canvas>();
                votingCanvas.renderMode = discussionCanvas.renderMode;
                votingCanvas.sortingOrder = discussionCanvas.sortingOrder;
                
                var canvasScaler = discussionUI.GetComponent<UnityEngine.UI.CanvasScaler>();
                if (canvasScaler != null)
                {
                    var newScaler = votingUIContainer.AddComponent<UnityEngine.UI.CanvasScaler>();
                    newScaler.uiScaleMode = canvasScaler.uiScaleMode;
                    newScaler.referenceResolution = canvasScaler.referenceResolution;
                    newScaler.matchWidthOrHeight = canvasScaler.matchWidthOrHeight;
                }
                
                var graphicRaycaster = discussionUI.GetComponent<UnityEngine.UI.GraphicRaycaster>();
                if (graphicRaycaster != null)
                {
                    votingUIContainer.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }
            }
            
            // Move board children (MainCardAnchor) to board canvas
            foreach (string childName in boardChildren)
            {
                Transform child = FindDeepChild(discussionUI.transform, childName);
                if (child != null)
                {
                    child.SetParent(boardCanvasRoot.transform, true);
                    Debug.Log($"[GameScreenUISetup]   ✅ BOARD: Moved {childName}");
                }
                else
                {
                    Debug.LogWarning($"[GameScreenUISetup]   ⚠️ Could not find {childName} in {discussionUIName}");
                }
            }
            
            // Move remaining children (voting buttons) to player canvas
            // Move all direct children that weren't moved to board
            Transform[] children = new Transform[discussionUI.transform.childCount];
            for (int i = 0; i < discussionUI.transform.childCount; i++)
            {
                children[i] = discussionUI.transform.GetChild(i);
            }
            
            foreach (Transform child in children)
            {
                // Only move if it wasn't already moved to board
                if (child != null && child.parent == discussionUI.transform)
                {
                    child.SetParent(votingUIContainer.transform, true);
                    Debug.Log($"[GameScreenUISetup]   ✅ PLAYER: Moved {child.name}");
                }
            }
            
            // Destroy the now-empty DiscussionUI GameObject
            Destroy(discussionUI);
            Debug.Log($"[GameScreenUISetup] ♻️ Destroyed empty {discussionUIName}");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] ⚠️ Could not find {discussionUIName}");
        }

        // === BOARD CANVAS (Shared Screen) ===
        if (resultsUI != null)
        {
            resultsUI.transform.SetParent(boardCanvasRoot.transform, true);
            Debug.Log($"[GameScreenUISetup] ✅ BOARD: Moved {resultsUIName}");
        }
        else
        {
            Debug.LogWarning($"[GameScreenUISetup] ⚠️ Could not find {resultsUIName}");
        }

        // Assign to UIModeSwitcher
        if (uiModeSwitcher != null)
        {
            var boardField = typeof(UIModeSwitcher).GetField("boardCanvasRoot", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var playerField = typeof(UIModeSwitcher).GetField("playerCanvasRoot", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (boardField != null)
                boardField.SetValue(uiModeSwitcher, boardCanvasRoot);
            
            if (playerField != null)
                playerField.SetValue(uiModeSwitcher, playerCanvasRoot);
            
            Debug.Log("[GameScreenUISetup] ✅ Connected to UIModeSwitcher");
        }
        else
        {
            Debug.LogError("[GameScreenUISetup] ❌ UIModeSwitcher not assigned!");
        }

        Debug.Log("[GameScreenUISetup] 🎯 Setup Complete!");
        Debug.Log("[GameScreenUISetup] G = Board (card only) | T = Tablet (buttons + hand)");
        Debug.Log("[GameScreenUISetup] ============================================");
    }

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
