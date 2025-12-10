using UnityEngine;
using TMPro;

public class BoardUIController : MonoBehaviour
{
    [Header("Text fields")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private TMP_Text crisisText;
    [SerializeField] private TMP_Text logText;

    private void Awake()
    {
        if (logText != null)
        {
            logText.text = "";
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

    // Called by the NextPhaseButton
    public void OnNextPhaseButtonClicked()
    {
        Debug.Log("[BoardUI] Next Phase button clicked!");
        
        // Forward to BoardController
        if (BoardController.LocalInstance != null)
        {
            BoardController.LocalInstance.OnNextPhaseButtonClicked();
        }
        else
        {
            Debug.LogWarning("[BoardUI] BoardController.LocalInstance is null!");
        }
    }
}
