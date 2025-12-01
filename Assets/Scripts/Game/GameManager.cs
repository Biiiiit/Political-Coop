// Scripts/Game/GameManager.cs
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly Dictionary<ulong, Role> _playerRoles = new();
    private readonly Queue<Role> _availableRoles = new();

    public BoardState BoardState { get; private set; }

    private void Awake()
    {
        // Simple singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize roles queue
        _availableRoles.Enqueue(Role.Farming);
        _availableRoles.Enqueue(Role.Industry);
        _availableRoles.Enqueue(Role.Housing);
        _availableRoles.Enqueue(Role.Nature);

        BoardState = new BoardState(turnNumber: 1, crisisLevel: 0);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // only host/server does this

        // Subscribe to Netcode events
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
        // Host itself also triggers this; we treat host as "board", not a role player
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
        Debug.Log($"[GameManager] Assigned {roleToAssign} to client {clientId}");

        // Tell that specific client their role
        SendRoleToClientClientRpc(roleToAssign, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        });

        // Optionally broadcast board state once they join
        BroadcastBoardStateToClient(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (_playerRoles.TryGetValue(clientId, out var role))
        {
            Debug.Log($"[GameManager] Client {clientId} with role {role} disconnected.");
            _playerRoles.Remove(clientId);

            if (role != Role.None)
            {
                _availableRoles.Enqueue(role); // return role to pool
            }
        }
    }

    [ClientRpc]
    private void SendRoleToClientClientRpc(Role role, ClientRpcParams clientRpcParams = default)
    {
        // This runs on that specific client.
        if (PlayerRoleController.LocalInstance != null)
        {
            PlayerRoleController.LocalInstance.SetRole(role);
        }
        else
        {
            Debug.LogWarning("[GameManager] LocalInstance of PlayerRoleController is null when assigning role.");
        }
    }

    private void BroadcastBoardStateToClient(ulong clientId)
    {
        BroadcastBoardStateClientRpc(BoardState.TurnNumber, BoardState.CrisisLevel, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        });
    }

    [ClientRpc]
    private void BroadcastBoardStateClientRpc(int turnNumber, int crisisLevel, ClientRpcParams clientRpcParams = default)
    {
        // Called on every client chosen in clientRpcParams (or all, if none).
        Debug.Log($"[Client] Board state updated: Turn {turnNumber}, Crisis {crisisLevel}");

        // BoardController can react to this if it exists
        if (BoardController.LocalInstance != null)
        {
            BoardController.LocalInstance.OnBoardStateUpdated(turnNumber, crisisLevel);
        }
    }

    // Example: advance turn from host/board
    [ServerRpc(RequireOwnership = false)]
    public void AdvanceTurnServerRpc()
    {
        BoardState.TurnNumber++;
        BoardState.CrisisLevel++; // simple fake logic

        Debug.Log($"[GameManager] Turn -> {BoardState.TurnNumber}, Crisis -> {BoardState.CrisisLevel}");

        // Send to all clients
        BroadcastBoardStateClientRpc(BoardState.TurnNumber, BoardState.CrisisLevel);
    }
}
