using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class VotingManager : NetworkBehaviour
{
    public static VotingManager Instance;

    public NetworkList<CardProposalNetwork> Proposals;
    private Dictionary<int, Dictionary<ulong, bool>> votes =
        new Dictionary<int, Dictionary<ulong, bool>>();

    private void Awake()
    {
        Instance = this;
        Proposals = new NetworkList<CardProposalNetwork>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CreatePlaceholderProposals();
        }
    }

    void CreatePlaceholderProposals()
    {
        Proposals.Add(new CardProposalNetwork
        {
            proposalId = 0,
            cardTitle = "Build Water Plant",
            proposerClientId = 0
        });

        Proposals.Add(new CardProposalNetwork
        {
            proposalId = 1,
            cardTitle = "Subsidize Farmers",
            proposerClientId = 1
        });
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitVoteServerRpc(int proposalId, bool vote, ulong voterId)
    {
        if (!votes.ContainsKey(proposalId))
        {
            votes[proposalId] = new Dictionary<ulong, bool>();
        }

        votes[proposalId][voterId] = vote;

        Debug.Log($"Vote received Proposal {proposalId} | Voter {voterId} | Vote {vote}");
    }
}
