using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // clientId -> role
    private readonly Dictionary<ulong, Role> _playerRoles = new();
    // clientId -> sector state
    private readonly Dictionary<ulong, SectorState> _sectorStates = new();
    // queue of available roles
    private readonly Queue<Role> _availableRoles = new();

    public BoardState BoardState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // fill role queue
        _availableRoles.Enqueue(Role.Farming);
        _availableRoles.Enqueue(Role.Industry);
        _availableRoles.Enqueue(Role.Housing);
        _availableRoles.Enqueue(Role.Nature);

        BoardState = new BoardState(turnNumber: 1, crisisLevel: 0, phase: Phase.Lobby);
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

    private void OnClientConnected(ulong clientId)
    {
        // Host itself also triggers this; treat host as board, not role player
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

        // tell that specific client its role
        SendRoleToClientClientRpc(roleToAssign, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        });

        // send initial board + sector state to that client
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

        if (BoardController.LocalInstance != null)
        {
            BoardController.LocalInstance.OnBoardStateUpdated(turnNumber, crisisLevel, phase);
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

    // Called by tablets via PlayerRoleController.RequestPlayCard
    [ServerRpc(RequireOwnership = false)]
    public void PlayCardServerRpc(ulong clientId, string cardId)
    {
        if (!_playerRoles.TryGetValue(clientId, out var role))
        {
            Debug.LogWarning($"[GameManager] Unknown client {clientId} tried to play a card.");
            return;
        }

        Debug.Log($"[GameManager] Client {clientId} ({role}) played card {cardId}");

        // Example: card effect -> global + per-role
        BoardState.CrisisLevel++;

        if (_sectorStates.TryGetValue(clientId, out var sector))
        {
            sector.ResourceLevel++;
            Debug.Log($"[GameManager] {role} resource now {sector.ResourceLevel}");

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

        // notify everybody about global board change
        BroadcastBoardStateClientRpc(
            BoardState.TurnNumber,
            BoardState.CrisisLevel,
            (int)BoardState.Phase);
    }

    // Host advances to next phase / turn
    [ServerRpc(RequireOwnership = false)]
    public void NextPhaseServerRpc()
    {
        switch (BoardState.Phase)
        {
            case Phase.Lobby:
                BoardState.Phase = Phase.Draw;
                break;
            case Phase.Draw:
                BoardState.Phase = Phase.Play;
                break;
            case Phase.Play:
                BoardState.Phase = Phase.Vote;
                break;
            case Phase.Vote:
                BoardState.Phase = Phase.Resolve;
                break;
            case Phase.Resolve:
                BoardState.Phase = Phase.Draw;
                BoardState.TurnNumber++;
                break;
        }

        Debug.Log($"[GameManager] Phase -> {BoardState.Phase}, Turn -> {BoardState.TurnNumber}");

        BroadcastBoardStateClientRpc(
            BoardState.TurnNumber,
            BoardState.CrisisLevel,
            (int)BoardState.Phase);
    }
}
