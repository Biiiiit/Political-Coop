using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to automatically set up UI elements in Screens.unity scene.
/// Attach this to the Canvas in Screens.unity and run Setup methods.
/// </summary>
public class ScreensSceneSetup : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string instructions = 
        "1. Open Screens.unity\n" +
        "2. Find BoardCanvas and attach this script\n" +
        "3. Click 'Setup Board UI' in context menu\n" +
        "4. Find PlayerCanvas and attach this script\n" +
        "5. Click 'Setup Player UI' in context menu\n" +
        "6. Assign created elements to BoardUIController/PlayerUIController";

    [Header("Created Objects (Auto-assigned)")]
    public GameObject waitingPanel;
    public TextMeshProUGUI waitingMessageText;
    public Button nextPhaseButton;
    public GameObject voteButtonsPanel;
    public Button voteYesButton;
    public Button voteNoButton;
    public GameObject resultsPanel;
    public TextMeshProUGUI resultsText;

    [ContextMenu("Setup Board UI")]
    public void SetupBoardUI()
    {
        Debug.Log("[ScreensSceneSetup] Setting up Board UI...");

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[ScreensSceneSetup] Must be attached to a Canvas!");
            return;
        }

        // Create Waiting Panel
        waitingPanel = CreateWaitingPanel(canvas.transform);

        // Create Next Phase Button
        nextPhaseButton = CreateNextPhaseButton(canvas.transform);

        Debug.Log("[ScreensSceneSetup] Board UI setup complete!");
        Debug.Log("[ScreensSceneSetup] Assign these to BoardUIController:");
        Debug.Log("  - waitingPanel -> " + waitingPanel.name);
        Debug.Log("  - waitingMessageText -> " + waitingMessageText.name);
        Debug.Log("  - nextPhaseButton -> " + nextPhaseButton.name);
    }

    [ContextMenu("Setup Player UI")]
    public void SetupPlayerUI()
    {
        Debug.Log("[ScreensSceneSetup] Setting up Player UI...");

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[ScreensSceneSetup] Must be attached to a Canvas!");
            return;
        }

        // Create Vote Buttons Panel
        voteButtonsPanel = CreateVoteButtonsPanel(canvas.transform);

        // Create Results Panel
        resultsPanel = CreateResultsPanel(canvas.transform);

        Debug.Log("[ScreensSceneSetup] Player UI setup complete!");
        Debug.Log("[ScreensSceneSetup] Assign these to PlayerUIController:");
        Debug.Log("  - voteButtonsPanel -> " + voteButtonsPanel.name);
        Debug.Log("  - voteYesButton -> " + voteYesButton.name);
        Debug.Log("  - voteNoButton -> " + voteNoButton.name);
        Debug.Log("  - resultsPanel -> " + resultsPanel.name);
    }

    private GameObject CreateWaitingPanel(Transform parent)
    {
        // Check if already exists
        Transform existing = parent.Find("WaitingPanel");
        if (existing != null)
        {
            Debug.Log("[ScreensSceneSetup] WaitingPanel already exists");
            waitingMessageText = existing.GetComponentInChildren<TextMeshProUGUI>();
            return existing.gameObject;
        }

        // Create panel
        GameObject panel = new GameObject("WaitingPanel");
        panel.transform.SetParent(parent, false);

        // Add RectTransform and configure
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Add Image for background
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // Create text child
        GameObject textObj = new GameObject("WaitingMessageText");
        textObj.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(800, 200);

        waitingMessageText = textObj.AddComponent<TextMeshProUGUI>();
        waitingMessageText.text = "Waiting...";
        waitingMessageText.fontSize = 48;
        waitingMessageText.alignment = TextAlignmentOptions.Center;
        waitingMessageText.color = Color.white;

        Debug.Log("[ScreensSceneSetup] Created WaitingPanel");
        return panel;
    }

    private Button CreateNextPhaseButton(Transform parent)
    {
        // Check if already exists
        Transform existing = parent.Find("NextPhaseButton");
        if (existing != null)
        {
            Debug.Log("[ScreensSceneSetup] NextPhaseButton already exists");
            return existing.GetComponent<Button>();
        }

        // Create button
        GameObject buttonObj = new GameObject("NextPhaseButton");
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-20, 20);
        rect.sizeDelta = new Vector2(200, 60);

        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 1f);

        Button btn = buttonObj.AddComponent<Button>();

        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Next Phase";
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        Debug.Log("[ScreensSceneSetup] Created NextPhaseButton");
        Debug.Log("[ScreensSceneSetup] IMPORTANT: Set Button OnClick -> BoardUIController.OnNextPhaseButtonClicked()");
        
        return btn;
    }

    private GameObject CreateVoteButtonsPanel(Transform parent)
    {
        // Check if already exists
        Transform existing = parent.Find("VoteButtonsPanel");
        if (existing != null)
        {
            Debug.Log("[ScreensSceneSetup] VoteButtonsPanel already exists");
            Button[] buttons = existing.GetComponentsInChildren<Button>(true);
            voteYesButton = buttons.Length > 0 ? buttons[0] : null;
            voteNoButton = buttons.Length > 1 ? buttons[1] : null;
            return existing.gameObject;
        }

        // Create panel
        GameObject panel = new GameObject("VoteButtonsPanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600, 200);

        // Create Yes button
        voteYesButton = CreateVoteButton(panel.transform, "VoteYesButton", "YES", new Color(0.2f, 0.8f, 0.2f), new Vector2(-160, 0));

        // Create No button
        voteNoButton = CreateVoteButton(panel.transform, "VoteNoButton", "NO", new Color(0.8f, 0.2f, 0.2f), new Vector2(160, 0));

        panel.SetActive(false); // Start inactive

        Debug.Log("[ScreensSceneSetup] Created VoteButtonsPanel");
        Debug.Log("[ScreensSceneSetup] IMPORTANT: Set button OnClicks:");
        Debug.Log("  - VoteYesButton -> PlayerUIController.OnVoteYesButtonClicked()");
        Debug.Log("  - VoteNoButton -> PlayerUIController.OnVoteNoButtonClicked()");

        return panel;
    }

    private Button CreateVoteButton(Transform parent, string name, string label, Color color, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(250, 120);

        Image img = buttonObj.AddComponent<Image>();
        img.color = color;

        Button btn = buttonObj.AddComponent<Button>();

        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold;

        return btn;
    }

    private GameObject CreateResultsPanel(Transform parent)
    {
        // Check if already exists
        Transform existing = parent.Find("ResultsPanel");
        if (existing != null)
        {
            Debug.Log("[ScreensSceneSetup] ResultsPanel already exists");
            resultsText = existing.GetComponentInChildren<TextMeshProUGUI>();
            return existing.gameObject;
        }

        // Create panel
        GameObject panel = new GameObject("ResultsPanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        // Create text
        GameObject textObj = new GameObject("ResultsText");
        textObj.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(900, 600);

        resultsText = textObj.AddComponent<TextMeshProUGUI>();
        resultsText.text = "Results will appear here";
        resultsText.fontSize = 36;
        resultsText.alignment = TextAlignmentOptions.Center;
        resultsText.color = Color.white;

        panel.SetActive(false); // Start inactive

        Debug.Log("[ScreensSceneSetup] Created ResultsPanel");
        return panel;
    }
}
