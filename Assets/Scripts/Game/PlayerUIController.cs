using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController LocalInstance { get; private set; }

    [Header("Text fields")]
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text infoText;

    [Header("Buttons")]
    [SerializeField] private Button playCardButton;
    [SerializeField] private Button voteYesButton;
    [SerializeField] private Button voteNoButton;

    [Header("Panels")]
    [SerializeField] private GameObject voteButtonsPanel;
    [SerializeField] private GameObject resultsPanel;

    private string currentSelectedCardId;
    private string currentVotingCardId;

    private void Awake()
    {
        LocalInstance = this;
        Debug.Log("[PlayerUIController] Instance created");
    }

    private void OnDestroy()
    {
        if (LocalInstance == this)
        {
            LocalInstance = null;
        }
    }

    public void SetRole(Role role)
    {
        if (roleText != null)
        {
            roleText.text = $"Role: {role}";
        }
    }

    public void UpdateSectorState(Role role, int resourceLevel)
    {
        if (resourceText != null)
        {
            resourceText.text = $"Resources: {resourceLevel}";
        }
    }

    public void ShowInfo(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
        }
    }

    public void HideAllPhaseUI()
    {
        if (voteButtonsPanel != null) voteButtonsPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (playCardButton != null) playCardButton.gameObject.SetActive(false);
    }

    public void ShowVoteButtons(bool show)
    {
        if (voteButtonsPanel != null)
        {
            voteButtonsPanel.SetActive(show);
        }
        
        if (voteYesButton != null) voteYesButton.gameObject.SetActive(show);
        if (voteNoButton != null) voteNoButton.gameObject.SetActive(show);
    }

    public void ShowResults()
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }
        
        Debug.Log("[PlayerUIController] Showing results");
    }

    // ===== Button Handlers =====

    public void OnPlayCardButtonClicked()
    {
        Debug.Log("[PlayerUIController] Play card button clicked");
        
        currentSelectedCardId = GetSelectedCardFromHand();
        
        if (string.IsNullOrEmpty(currentSelectedCardId))
        {
            Debug.LogWarning("[PlayerUIController] No card selected");
            return;
        }
        
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnPlayerPlayCard(currentSelectedCardId);
        }
        else
        {
            Debug.LogWarning("[PlayerUIController] GameFlowManager not available");
        }
    }

    public void OnVoteYesButtonClicked()
    {
        Debug.Log("[PlayerUIController] Vote YES clicked");
        
        if (GameFlowManager.Instance != null && !string.IsNullOrEmpty(currentVotingCardId))
        {
            GameFlowManager.Instance.OnPlayerVote(currentVotingCardId, true);
        }
        else
        {
            Debug.LogWarning("[PlayerUIController] Cannot vote - no card or no GameFlowManager");
        }
    }

    public void OnVoteNoButtonClicked()
    {
        Debug.Log("[PlayerUIController] Vote NO clicked");
        
        if (GameFlowManager.Instance != null && !string.IsNullOrEmpty(currentVotingCardId))
        {
            GameFlowManager.Instance.OnPlayerVote(currentVotingCardId, false);
        }
        else
        {
            Debug.LogWarning("[PlayerUIController] Cannot vote - no card or no GameFlowManager");
        }
    }

    // ===== Helper Methods =====

    private string GetSelectedCardFromHand()
    {
        // TODO: Implement based on your card hand UI
        // This should return the cardId of the currently selected card
        Debug.LogWarning("[PlayerUIController] GetSelectedCardFromHand not implemented yet");
        return "card_placeholder_id";
    }

    public void SetCurrentVotingCard(string cardId)
    {
        currentVotingCardId = cardId;
        Debug.Log($"[PlayerUIController] Current voting card set to: {cardId}");
    }

    // Called by GameManager via RPC (from PlayerRoleController)
    public void OnCardsToVoteOn(string cardsCombined)
    {
        Debug.Log($"[PlayerUIController] Cards to vote on: {cardsCombined}");
        
        if (!string.IsNullOrEmpty(cardsCombined))
        {
            string[] cardIds = cardsCombined.Split('|');
            if (cardIds.Length > 0)
            {
                currentVotingCardId = cardIds[0];
            }
        }
    }
}
