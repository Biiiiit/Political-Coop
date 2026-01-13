using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardUIController : MonoBehaviour
{
    public static BoardUIController LocalInstance { get; private set; }

    [Header("Text fields")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private TMP_Text crisisText;
    [SerializeField] private TMP_Text logText;

    [Header("UI Panels")]
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private TMP_Text waitingMessageText;

    [Header("Buttons")]
    [SerializeField] private Button nextPhaseButton;

    private void Awake()
    {
        LocalInstance = this;
        
        if (logText != null)
        {
            logText.text = "";
        }
        
        Debug.Log("[BoardUIController] Instance created");
    }

    private void OnDestroy()
    {
        if (LocalInstance == this)
        {
            LocalInstance = null;
        }
    }

    public void UpdateBoard(int turnNumber, int crisisLevel, Phase phase)
    {
        if (turnText != null)
            turnText.text = $"Turn: {turnNumber}";

        if (phaseText != null)
            phaseText.text = $"Phase: {phase}";

        if (crisisText != null)
            crisisText.text = $"Crisis: {crisisLevel}";
    }

    public void AddLogMessage(string message)
    {
        if (logText == null) return;

        logText.text = message;
        Debug.Log("[BoardUI] " + message);
    }

    // New methods for GameFlowManager integration
    public void ShowWaitingScreen(string message)
    {
        if (waitingPanel != null)
        {
            waitingPanel.SetActive(true);
        }
        
        if (waitingMessageText != null)
        {
            waitingMessageText.text = message;
        }
        
        Debug.Log($"[BoardUIController] Waiting screen: {message}");
    }

    public void HideWaitingScreen()
    {
        if (waitingPanel != null)
        {
            waitingPanel.SetActive(false);
        }
        
        Debug.Log("[BoardUIController] Hiding waiting screen");
    }

    public void EnableNextPhase(bool enabled)
    {
        if (nextPhaseButton != null)
        {
            nextPhaseButton.interactable = enabled;
        }
    }

    // Called by the NextPhaseButton
    public void OnNextPhaseButtonClicked()
    {
        Debug.Log("[BoardUIController] Next Phase button clicked!");
        
        // Forward to GameFlowManager if available, otherwise BoardController
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnNextPhaseButtonClicked();
        }
        else if (BoardController.LocalInstance != null)
        {
            BoardController.LocalInstance.OnNextPhaseButtonClicked();
        }
        else
        {
            Debug.LogWarning("[BoardUIController] No controller available to handle button click!");
        }
    }
}
