using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Dual Screen Setup")]
    [SerializeField] private GameObject dualScreenNetworkManagerPrefab;

    // clientId -> role
    private readonly Dictionary<ulong, Role> _playerRoles = new();
    // clientId -> sector state
    private readonly Dictionary<ulong, SectorState> _sectorStates = new();
    // queue of available roles
    private readonly Queue<Role> _availableRoles = new();

    // ====== TURN / PHASE STATE ======
    public BoardState BoardState { get; private set; }

    // One simple record for a played card this turn
    private class PlayedCard
    {
        public string CardId;
        public ulong ClientId;
        public Role Role;

        public PlayedCard(string cardId, ulong clientId, Role role)
        {
            CardId = cardId;
            ClientId = clientId;
            Role = role;
        }
    }

    // All cards played in the current turn during Play phase
    private readonly List<PlayedCard> _playedCardsThisTurn = new();

    // Set of players who have already played a card this turn
    private readonly HashSet<ulong> _playersWhoPlayedThisTurn = new();

    // Votes: cardId -> (clientId -> voteYes?)
    private readonly Dictionary<string, Dictionary<ulong, bool>> _votesByCard = new();

    // Convenience
    private int PlayerCount => _playerRoles.Count;

    // =========================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Fill role queue
        _availableRoles.Enqueue(Role.Farming);
        _availableRoles.Enqueue(Role.Industry);
        _availableRoles.Enqueue(Role.Housing);
        _availableRoles.Enqueue(Role.Nature);

        // Initial board state
        BoardState = new BoardState(
            turnNumber: 1,
            crisisLevel: 0,
            phase: Phase.Lobby
        );
    }

    void Start()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            if (dualScreenNetworkManagerPrefab != null)
            {
                var instance = Instantiate(dualScreenNetworkManagerPrefab);
                var networkObject = instance.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Spawn();
                    Debug.Log("[GameManager] DualScreenNetworkManager spawned");
                }
                else
                {
                    Debug.LogError("[GameManager] DualScreenNetworkManager prefab missing NetworkObject component");
                }
            }
            else
            {
                Debug.LogWarning("[GameManager] DualScreenNetworkManager prefab not assigned in Inspector");
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        Debug.Log("[GameManager] NetworkSpawn on server.");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    // ================== CONNECTIONS & ROLES ===================

    private void OnClientConnected(ulong clientId)
    {
        // Host is also a client; treat host as board, not a role player
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"[GameManager] Host connected as client {clientId} (board).");
            return;
        }

        Role roleToAssign = Role.None;
        if (_availableRoles.Count > 0)
        {
            roleToAssign = _availableRoles.Dequeue();
        }

        _playerRoles[clientId] = roleToAssign;
        _sectorStates[clientId] = new SectorState(roleToAssign);

        Debug.Log($"[GameManager] Assigned {roleToAssign} to client {clientId}");

        // Tell that specific client their role
        SendRoleToClientClientRpc(roleToAssign, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        });

        // Send initial board + sector state to that client
        BroadcastBoardStateToClient(clientId);

        var sector = _sectorStates[clientId];
        SendSectorStateToClientClientRpc(
            sector.Role,
            sector.ResourceLevel,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { clientId }
                }
            });
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (_playerRoles.TryGetValue(clientId, out var role))
        {
            Debug.Log($"[GameManager] Client {clientId} with role {role} disconnected.");
            _playerRoles.Remove(clientId);

            if (role != Role.None)
            {
                _availableRoles.Enqueue(role);
            }
        }

        if (_sectorStates.ContainsKey(clientId))
        {
            _sectorStates.Remove(clientId);
        }

        // Clean up any mid-turn data related to this client
        _playersWhoPlayedThisTurn.Remove(clientId);
        foreach (var kvp in _votesByCard)
        {
            kvp.Value.Remove(clientId);
        }
    }

    [ClientRpc]
    private void SendRoleToClientClientRpc(Role role, ClientRpcParams clientRpcParams = default)
    {
        if (PlayerRoleController.LocalInstance != null)
        {
            PlayerRoleController.LocalInstance.SetRole(role);
        }
        else
        {
            Debug.LogWarning("[GameManager] Local PlayerRoleController not present when assigning role.");
        }
    }

    // ================== BOARD STATE BROADCASTING ===================

    private void BroadcastBoardStateToClient(ulong clientId)
    {
        BroadcastBoardStateClientRpc(
            BoardState.TurnNumber,
            BoardState.CrisisLevel,
            (int)BoardState.Phase,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { clientId }
                }
            });
    }

    [ClientRpc]
    private void BroadcastBoardStateClientRpc(
        int turnNumber,
        int crisisLevel,
        int phaseInt,
        ClientRpcParams clientRpcParams = default)
    {
        Phase phase = (Phase)phaseInt;

        Debug.Log($"[Client] Board state: Turn {turnNumber}, Crisis {crisisLevel}, Phase {phase}");

        // Board (host)
        if (BoardController.LocalInstance != null)
        {
            BoardController.LocalInstance.OnBoardStateUpdated(turnNumber, crisisLevel, phase);
        }

        // Player (tablet)
        if (PlayerRoleController.LocalInstance != null)
        {
            PlayerRoleController.LocalInstance.OnBoardStateUpdated(turnNumber, crisisLevel, phase);
        }
    }

    [ClientRpc]
    private void SendSectorStateToClientClientRpc(
        Role role,
        int resourceLevel,
        ClientRpcParams clientRpcParams = default)
    {
        if (PlayerRoleController.LocalInstance != null)
        {
            PlayerRoleController.LocalInstance.OnSectorStateUpdated(role, resourceLevel);
        }
    }

    // Cards to vote on → broadcast as a single string "id1|id2|id3"
    [ClientRpc]
    private void BroadcastCardsToVoteOnClientRpc(
        string cardsCombined,
        ClientRpcParams clientRpcParams = default)
    {
        if (PlayerRoleController.LocalInstance != null)
        {
            PlayerRoleController.LocalInstance.OnCardsToVoteOn(cardsCombined);
        }
    }

    // ================== SERVER-SIDE HELPERS ===================

    private void SetPhase(Phase newPhase)
    {
        BoardState.Phase = newPhase;
        Debug.Log($"[GameManager] Phase set to {newPhase}");
        BroadcastBoardStateClientRpc(BoardState.TurnNumber, BoardState.CrisisLevel, (int)BoardState.Phase);
    }

    private void ClearTurnRuntimeData()
    {
        _playedCardsThisTurn.Clear();
        _playersWhoPlayedThisTurn.Clear();
        _votesByCard.Clear();
    }

    private bool AllPlayersHavePlayed()
    {
        if (PlayerCount == 0) return false;
        return _playersWhoPlayedThisTurn.Count >= PlayerCount;
    }

    private bool AllVotesComplete()
    {
        if (PlayerCount == 0) return false;
        if (_playedCardsThisTurn.Count == 0) return true; // nothing to vote on

        foreach (var played in _playedCardsThisTurn)
        {
            if (!_votesByCard.TryGetValue(played.CardId, out var votesForCard))
            {
                return false;
            }

            if (votesForCard.Count < PlayerCount)
            {
                return false;
            }
        }

        return true;
    }

    // ================== PUBLIC SERVER RPCs (ACTIONS) ===================

    // Called by tablets when they "play a card" during Play phase
    [ServerRpc(RequireOwnership = false)]
    public void PlayCardServerRpc(ulong clientId, string cardId)
    {
        if (!_playerRoles.TryGetValue(clientId, out var role))
        {
            Debug.LogWarning($"[GameManager] Unknown client {clientId} tried to play a card.");
            return;
        }

        if (BoardState.Phase != Phase.Play)
        {
            Debug.Log($"[GameManager] Ignoring PlayCard from {clientId} because phase is {BoardState.Phase}.");
            return;
        }

        if (_playersWhoPlayedThisTurn.Contains(clientId))
        {
            Debug.Log($"[GameManager] Client {clientId} already played a card this turn.");
            return;
        }

        var played = new PlayedCard(cardId, clientId, role);
        _playedCardsThisTurn.Add(played);
        _playersWhoPlayedThisTurn.Add(clientId);

        Debug.Log($"[GameManager] Client {clientId} ({role}) played card {cardId}.");

        // Simple effect stub (later: real card effects)
        BoardState.CrisisLevel++;
        BroadcastBoardStateClientRpc(BoardState.TurnNumber, BoardState.CrisisLevel, (int)BoardState.Phase);

        // Initialize vote container for this card
        if (!_votesByCard.ContainsKey(cardId))
        {
            _votesByCard[cardId] = new Dictionary<ulong, bool>();
        }

        // Auto-move to Vote if everyone has played
        if (AllPlayersHavePlayed())
        {
            AdvanceFromPlayToVote();
        }
    }

    // Called by tablets when they vote on a specific card during Vote phase
    [ServerRpc(RequireOwnership = false)]
    public void VoteOnCardServerRpc(ulong clientId, string cardId, bool voteYes)
    {
        if (BoardState.Phase != Phase.Vote)
        {
            Debug.Log($"[GameManager] Ignoring Vote from {clientId} because phase is {BoardState.Phase}.");
            return;
        }

        if (!_votesByCard.TryGetValue(cardId, out var votesForCard))
        {
            Debug.LogWarning($"[GameManager] Vote received for unknown cardId {cardId}.");
            return;
        }

        votesForCard[clientId] = voteYes;
        Debug.Log($"[GameManager] Client {clientId} voted {(voteYes ? "YES" : "NO")} on card {cardId}.");

        if (AllVotesComplete())
        {
            AdvanceFromVoteToResolve();
        }
    }

    // Called by host (board) UI to manually advance phase
    [ServerRpc(RequireOwnership = false)]
    public void NextPhaseServerRpc()
    {
        Debug.Log($"[GameManager] NextPhase requested from phase {BoardState.Phase}");

        switch (BoardState.Phase)
        {
            case Phase.Lobby:
                StartNewGameFromLobby();
                break;

            case Phase.Draw:
                EnterPlayPhase();
                break;

            case Phase.Play:
                AdvanceFromPlayToVote();
                break;

            case Phase.Vote:
                AdvanceFromVoteToResolve();
                break;

            case Phase.Resolve:
                EndOfTurnAndGoToNext();
                break;
        }
    }

    // ================== PHASE TRANSITION IMPLEMENTATIONS ===================

    private void StartNewGameFromLobby()
    {
        Debug.Log("[GameManager] Starting game from Lobby -> Draw.");
        ClearTurnRuntimeData();
        BoardState.TurnNumber = 1;
        BoardState.CrisisLevel = 0;
        SetPhase(Phase.Draw);

        OnEnterDrawPhase();
    }

    private void OnEnterDrawPhase()
    {
        Debug.Log("[GameManager] Enter Draw phase.");
        // Later: deal cards per player here.
    }

    private void EnterPlayPhase()
    {
        Debug.Log("[GameManager] Draw -> Play.");
        ClearTurnRuntimeData();
        SetPhase(Phase.Play);
        // Later: enforce that each player must choose 1 card from their hand.
    }

    private void AdvanceFromPlayToVote()
    {
        Debug.Log("[GameManager] Play -> Vote.");
        SetPhase(Phase.Vote);

        // Prepare combined string of cardIds to vote on: "id1|id2|id3"
        string cardsCombined;
        if (_playedCardsThisTurn.Count == 0)
        {
            cardsCombined = string.Empty;
        }
        else
        {
            var ids = new List<string>(_playedCardsThisTurn.Count);
            foreach (var played in _playedCardsThisTurn)
            {
                ids.Add(played.CardId);
            }

            cardsCombined = string.Join("|", ids);
        }

        // Broadcast list of cards to each client so they know what to vote on
        BroadcastCardsToVoteOnClientRpc(cardsCombined);

        // _votesByCard already initialized in PlayCardServerRpc
    }

    private void AdvanceFromVoteToResolve()
    {
        Debug.Log("[GameManager] Vote -> Resolve.");
        SetPhase(Phase.Resolve);

        // Immediately resolve in this simple version
        ResolveCardsAndEndTurn();
    }

    private void ResolveCardsAndEndTurn()
    {
        Debug.Log("[GameManager] Resolving cards...");

        foreach (var played in _playedCardsThisTurn)
        {
            bool accepted = true;

            if (_votesByCard.TryGetValue(played.CardId, out var votesForCard))
            {
                int yesCount = 0;
                int noCount = 0;
                foreach (var v in votesForCard.Values)
                {
                    if (v) yesCount++; else noCount++;
                }

                accepted = yesCount >= noCount;
            }

            Debug.Log($"[GameManager] Card {played.CardId} by {played.Role} " +
                      $"is {(accepted ? "ACCEPTED" : "REJECTED")}.");

            // Apply card effects if accepted
            if (accepted)
            {
                ApplyCardEffectsToBoard(played);
            }
        }

        EndOfTurnAndGoToNext();
    }

    private void ApplyCardEffectsToBoard(PlayedCard card)
    {
        // Get the card's assigned risks and add them to the board
        Debug.Log($"[GameManager] Applying card {card.CardId} effects from {card.Role}");

        if (BoardRiskManager.Instance == null)
        {
            Debug.LogWarning("[GameManager] BoardRiskManager.Instance not found - cannot apply card effects to board");
            return;
        }

        // TODO: Get the actual card GameObject and its Assignedrisks component
        // For now, this is a placeholder showing the integration point
        // In full implementation, you'd need to track card objects or pass card data
    }

    private void EndOfTurnAndGoToNext()
    {
        Debug.Log("[GameManager] End of turn. Advancing to next turn.");

        // TODO later: calamity / risk system here

        BoardState.TurnNumber++;
        ClearTurnRuntimeData();

        // Next turn starts at Draw
        SetPhase(Phase.Draw);
        OnEnterDrawPhase();
    }
}
