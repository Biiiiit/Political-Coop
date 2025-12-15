using UnityEngine;
using Unity.Netcode;

public class VotingUI : MonoBehaviour
{
    public Transform proposalContainer;
    public GameObject proposalPrefab;

    void Start()
    {
        VotingManager.Instance.Proposals.OnListChanged += RefreshUI;
        RefreshUI(default);
    }

    void RefreshUI(NetworkListEvent<CardProposalNetwork> changeEvent)
    {
        foreach (Transform child in proposalContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var proposal in VotingManager.Instance.Proposals)
        {
            GameObject ui = Instantiate(proposalPrefab, proposalContainer);
            ui.GetComponent<ProposalUI>().Setup(proposal);
        }
    }
}
