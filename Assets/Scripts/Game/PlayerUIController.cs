// Assets/Scripts/Game/PlayerUIController.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("Main text fields")]
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text infoText;

    [Header("Board info (for this player)")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text crisisText;
    [SerializeField] private TMP_Text phaseText;

    [Header("Actions")]
    [SerializeField] private Button playCardButton;

    private void Start()
    {
        if (roleText != null) roleText.text = "Role: (not assigned)";
        if (resourceText != null) resourceText.text = "Resource: 0";
        if (infoText != null) infoText.text = "Waiting for role...";

        if (turnText != null) turnText.text = "Turn: -";
        if (crisisText != null) crisisText.text = "Crisis: -";
        if (phaseText != null) phaseText.text = "Phase: Lobby";

        if (playCardButton != null)
            playCardButton.interactable = false; // disabled until Play phase
    }

    public void SetRole(Role role)
    {
        if (roleText != null)
        {
            roleText.text = $"Role: {role}";
        }

        if (infoText != null)
        {
            infoText.text = "Wait for the Play phase to play cards.";
        }
    }

    public void UpdateSector(Role role, int resourceLevel)
    {
        if (resourceText != null)
        {
            resourceText.text = $"Resource ({role}): {resourceLevel}";
        }
    }

    public void UpdateBoardState(int turnNumber, int crisisLevel, Phase phase)
    {
        if (turnText != null)
            turnText.text = $"Turn: {turnNumber}";

        if (crisisText != null)
            crisisText.text = $"Crisis: {crisisLevel}";

        if (phaseText != null)
            phaseText.text = $"Phase: {phase}";

        UpdatePhaseHint(phase);
    }

    private void UpdatePhaseHint(Phase phase)
    {
        bool canPlay = false;
        string message;

        switch (phase)
        {
            case Phase.Lobby:
                message = "Lobby: waiting for the game to start.";
                canPlay = false;
                break;
            case Phase.Draw:
                message = "Draw: you get / discuss cards. No play yet.";
                canPlay = false;
                break;
            case Phase.Play:
                message = "Play: choose and play one card.";
                canPlay = true;
                break;
            case Phase.Vote:
                message = "Vote: discuss and vote on cards. No more plays.";
                canPlay = false;
                break;
            case Phase.Resolve:
                message = "Resolve: watch consequences.";
                canPlay = false;
                break;
            default:
                message = "Unknown phase.";
                canPlay = false;
                break;
        }

        if (infoText != null)
            infoText.text = message;

        if (playCardButton != null)
            playCardButton.interactable = canPlay;
    }

    // Called by PlayCardButton OnClick
    public void OnPlayCardButtonClicked()
    {
        if (PlayerRoleController.LocalInstance == null)
        {
            Debug.LogWarning("[PlayerUI] No local player to send PlayCard to.");
            return;
        }

        PlayerRoleController.LocalInstance.RequestPlayCard("FAKE_CARD_UI");
    }
}
