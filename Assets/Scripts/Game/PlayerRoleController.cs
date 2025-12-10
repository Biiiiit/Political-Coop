// Assets/Scripts/Game/PlayerRoleController.cs
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerRoleController : NetworkBehaviour
{
    public static PlayerRoleController LocalInstance { get; private set; }

    [SerializeField] private PlayerUIController playerUI;

    public Role Role { get; private set; } = Role.None;
    public int ResourceLevel { get; private set; }

    private Phase currentPhase = Phase.Lobby;

    // Cards this player needs to vote on during Vote phase
    private readonly List<string> cardsToVoteOn = new();

    private void Awake()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        LocalInstance = this;
        Debug.Log($"[Client {OwnerClientId}] PlayerRoleController spawned.");

        if (playerUI == null)
        {
            playerUI = FindObjectOfType<PlayerUIController>();
        }
    }

    public void SetRole(Role role)
    {
        if (!IsOwner) return;

        Role = role;
        Debug.Log($"[Client {OwnerClientId}] Assigned role: {Role}");

        if (playerUI != null)
        {
            playerUI.SetRole(role);
        }
    }

    public void OnSectorStateUpdated(Role role, int resourceLevel)
    {
        if (!IsOwner) return;

        ResourceLevel = resourceLevel;
        Debug.Log($"[Client {OwnerClientId}] Sector update: role={role}, resource={resourceLevel}");

        if (playerUI != null)
        {
            playerUI.UpdateSector(role, resourceLevel);
        }
    }

    public void OnBoardStateUpdated(int turnNumber, int crisisLevel, Phase phase)
    {
        if (!IsOwner) return;

        currentPhase = phase;
        Debug.Log($"[Client {OwnerClientId}] Board state for player: Turn {turnNumber}, Crisis {crisisLevel}, Phase {phase}");

        if (playerUI != null)
        {
            playerUI.UpdateBoardState(turnNumber, crisisLevel, phase);
        }

        // When leaving Vote phase, clear remaining pending votes (if any)
        if (phase != Phase.Vote && cardsToVoteOn.Count > 0)
        {
            cardsToVoteOn.Clear();
        }
    }

    // Called from GameManager when entering Vote phase
    // cardsCombined is "id1|id2|id3" or empty string if none
    public void OnCardsToVoteOn(string cardsCombined)
    {
        if (!IsOwner) return;

        cardsToVoteOn.Clear();

        if (!string.IsNullOrEmpty(cardsCombined))
        {
            string[] split = cardsCombined.Split('|');
            cardsToVoteOn.AddRange(split);
        }

        Debug.Log($"[Client {OwnerClientId}] Received cards to vote on: {string.Join(", ", cardsToVoteOn)}");

        if (playerUI != null)
        {
            playerUI.ShowVotePrompt(cardsToVoteOn);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Debug: press P in Play phase to "play a card" (useful on PC)
        if (Input.GetKeyDown(KeyCode.P) && currentPhase == Phase.Play)
        {
            RequestPlayCard("FAKE_CARD_P_KEY");
        }

        // Voting is done via UI buttons now
    }

    // Called from UI or from debug key in Play phase
    public void RequestPlayCard(string cardId)
    {
        if (currentPhase != Phase.Play)
        {
            Debug.Log($"[Client {OwnerClientId}] Not in Play phase, cannot play card.");
            return;
        }

        Debug.Log($"[Client {OwnerClientId}] Requesting to play card {cardId}");
        GameManager.Instance.PlayCardServerRpc(OwnerClientId, cardId);
    }

    // Called from PlayerUIController vote buttons
    public void VoteOnAllCardsFromUI(bool yes)
    {
        if (currentPhase != Phase.Vote)
        {
            Debug.Log($"[Client {OwnerClientId}] Not in Vote phase, ignoring vote.");
            return;
        }

        if (cardsToVoteOn.Count == 0)
        {
            Debug.Log($"[Client {OwnerClientId}] No cards to vote on.");
            return;
        }

        foreach (var cardId in cardsToVoteOn)
        {
            Debug.Log($"[Client {OwnerClientId}] Voting {(yes ? "YES" : "NO")} on {cardId}");
            GameManager.Instance.VoteOnCardServerRpc(OwnerClientId, cardId, yes);
        }

        cardsToVoteOn.Clear();

        if (playerUI != null)
        {
            playerUI.ShowAfterVoteMessage();
        }
    }
}
