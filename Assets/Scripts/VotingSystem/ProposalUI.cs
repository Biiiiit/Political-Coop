using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProposalUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text cardTitleNext;
    [SerializeField] private Button voteYesButton;
    [SerializeField] private Button voteNoButton;

    private CardProposalNetwork currentProposal;

    public void Setup(CardProposalNetwork proposal)
    {
        currentProposal = proposal;

        if (cardTitleNext != null)
            cardTitleNext.text = proposal.cardTitle.ToString();
        else
            Debug.LogWarning("cardTitleNext not assigned!");

        // Remove old listeners to avoid duplicates
        if (voteYesButton != null)
        {
            voteYesButton.onClick.RemoveAllListeners();
            voteYesButton.onClick.AddListener(OnVoteYes);
        }
        else Debug.LogWarning("voteYesButton not assigned!");

        if (voteNoButton != null)
        {
            voteNoButton.onClick.RemoveAllListeners();
            voteNoButton.onClick.AddListener(OnVoteNo);
        }
        else Debug.LogWarning("voteNoButton not assigned!");

        Debug.Log($"Setup called for proposal {proposal.proposalId}");
    }

    private void OnVoteYes()
    {
        Debug.Log($"Voted YES for proposal {currentProposal.proposalId}");
        // Voeg hier je netwerk-call toe
        // VotingManager.Instance.CastVote(currentProposal.proposalId, true);
    }

    private void OnVoteNo()
    {
        Debug.Log($"Voted NO for proposal {currentProposal.proposalId}");
        // VotingManager.Instance.CastVote(currentProposal.proposalId, false);
    }
}
