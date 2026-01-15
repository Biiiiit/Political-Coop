using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Displays waiting/idle UI on the shared screen while tablets are in GameLibrary or CardLibrary.
/// Shows messages like "Players are selecting cards..." etc.
/// </summary>
public class SharedScreenWaitingUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private TextMeshProUGUI waitingMessageText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Image loadingSpinner;

    [Header("Messages")]
    [SerializeField] private string gameLibraryMessage = "Players are selecting game topic...";
    [SerializeField] private string cardLibraryMessage = "Players are choosing cards...";
    [SerializeField] private string gameScreenMessage = "Players are playing their cards...";

    [Header("Animation")]
    [SerializeField] private float spinnerSpeed = 100f;

    private void Update()
    {
        // Rotate loading spinner
        if (loadingSpinner != null && loadingSpinner.gameObject.activeSelf)
        {
            loadingSpinner.transform.Rotate(0, 0, -spinnerSpeed * Time.deltaTime);
        }
    }

    public void ShowWaiting(string customMessage = null)
    {
        if (waitingPanel != null)
            waitingPanel.SetActive(true);

        if (waitingMessageText != null && !string.IsNullOrEmpty(customMessage))
            waitingMessageText.text = customMessage;
    }

    public void HideWaiting()
    {
        if (waitingPanel != null)
            waitingPanel.SetActive(false);
    }

    public void SetMessageForScreen(DualScreenManager.ScreenType screenType)
    {
        string message = screenType switch
        {
            DualScreenManager.ScreenType.GameLibrary => gameLibraryMessage,
            DualScreenManager.ScreenType.CardLibrary => cardLibraryMessage,
            DualScreenManager.ScreenType.GameScreen => gameScreenMessage,
            _ => "Waiting for players..."
        };

        ShowWaiting(message);
    }

    public void UpdatePlayerCount(int current, int total)
    {
        if (playerCountText != null)
        {
            playerCountText.text = $"Players: {current}/{total}";
        }
    }
}
