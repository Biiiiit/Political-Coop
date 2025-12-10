// Assets/Scripts/Game/PlayerUIController.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField] private Button voteYesButton;
    [SerializeField] private Button voteNoButton;

    private void Start()
    {
        if (roleText != null) roleText.text = "Role: (not assigned)";
        if (resourceText != null) resourceText.text = "Resource: 0";
        if (infoText != null) infoText.text = "Waiting for role...";

        if (turnText != null) turnText.text = "Turn: -";
        if (crisisText != null) crisisText.text = "Crisis: -";
        if (phaseText != null) phaseText.text = "Phase: Lobby";

        if (playCardButton != null) playCardButton.interactable = false;
        if (voteYesButton != null) voteYesButton.interactable = false;
        if (voteNoButton != null) voteNoButton.interactable = false;
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
        string message;

        switch (phase)
        {
            case Phase.Lobby:
                message = "Lobby: waiting for the game to start.";
                SetPlayInteractable(false);
                SetVoteInteractable(false);
                break;
            case Phase.Draw:
                message = "Draw: cards are being prepared / discussed.";
                SetPlayInteractable(false);
                SetVoteInteractable(false);
                break;
            case Phase.Play:
                message = "Play: choose and play one card.";
                SetPlayInteractable(true);
                SetVoteInteractable(false);
                break;
            case Phase.Vote:
                message = "Vote: discuss and vote on cards.";
                SetPlayInteractable(false);
                // We’ll enable vote buttons only if there are cards to vote on
                // (handled in ShowVotePrompt)
                break;
            case Phase.Resolve:
                message = "Resolve: watch the consequences.";
                SetPlayInteractable(false);
                SetVoteInteractable(false);
                break;
            default:
                message = "Unknown phase.";
                SetPlayInteractable(false);
                SetVoteInteractable(false);
                break;
        }

        if (infoText != null)
            infoText.text = message;
    }

    private void SetPlayInteractable(bool value)
    {
        if (playCardButton != null)
            playCardButton.interactable = value;
    }

    private void SetVoteInteractable(bool value)
    {
        if (voteYesButton != null)
            voteYesButton.interactable = value;
        if (voteNoButton != null)
            voteNoButton.interactable = value;
    }

    // Called from GameManager via PlayerRoleController when cards to vote on arrive
    public void ShowVotePrompt(List<string> cardIds)
    {
        if (cardIds.Count == 0)
        {
            if (infoText != null)
                infoText.text = "Vote phase: no cards to vote on.";
            SetVoteInteractable(false);
            return;
        }

        if (infoText != null)
        {
            infoText.text = $"Vote phase: you must vote on {cardIds.Count} card(s).";
        }

        // Only enable vote buttons if we’re actually in Vote phase
        SetVoteInteractable(true);
    }

    public void ShowAfterVoteMessage()
    {
        if (infoText != null)
        {
            infoText.text = "Vote submitted. Waiting for others...";
        }

        SetVoteInteractable(false);
    }

    // Button hooks
    public void OnPlayCardButtonClicked()
    {
        if (PlayerRoleController.LocalInstance == null)
        {
            Debug.LogWarning("[PlayerUI] No local player to send PlayCard to.");
            return;
        }

        PlayerRoleController.LocalInstance.RequestPlayCard("FAKE_CARD_UI");
    }

    public void OnVoteYesButtonClicked()
    {
        if (PlayerRoleController.LocalInstance == null)
        {
            Debug.LogWarning("[PlayerUI] No local player to send vote to.");
            return;
        }

        PlayerRoleController.LocalInstance.VoteOnAllCardsFromUI(true);
    }

    public void OnVoteNoButtonClicked()
    {
        if (PlayerRoleController.LocalInstance == null)
        {
            Debug.LogWarning("[PlayerUI] No local player to send vote to.");
            return;
        }

        PlayerRoleController.LocalInstance.VoteOnAllCardsFromUI(false);
    }
}
