using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Collections;

/// <summary>
/// Synchronizes game data from GameScreen (tablet) to GameBoard (shared screen).
/// When players finish playing cards on their tablets (GameScreen),
/// this sends the results to the shared screen (GameBoard) for display.
/// </summary>
public class GameScreenToGameBoardSync : NetworkBehaviour
{
    public static GameScreenToGameBoardSync Instance { get; private set; }

    [Header("Sync State")]
    private NetworkList<CardPlayData> playedCards;
    private NetworkVariable<bool> allPlayersReady = new NetworkVariable<bool>(false);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playedCards = new NetworkList<CardPlayData>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            playedCards.OnListChanged += OnPlayedCardsChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            playedCards.OnListChanged -= OnPlayedCardsChanged;
        }
        base.OnNetworkDespawn();
    }

    /// <summary>
    /// Called when a player plays a card on their tablet (GameScreen)
    /// </summary>
    public void ReportCardPlayed(string cardId, string cardTitle, ulong playerId)
    {
        if (!IsSpawned)
        {
            Debug.LogWarning("[GameScreenToGameBoardSync] Not spawned, cannot report card");
            return;
        }

        ReportCardPlayedServerRpc(cardId, cardTitle, playerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReportCardPlayedServerRpc(string cardId, string cardTitle, ulong playerId)
    {
        CardPlayData data = new CardPlayData(cardId, cardTitle, playerId, Time.time);

        playedCards.Add(data);
        Debug.Log($"[GameScreenToGameBoardSync] Card played: {cardTitle} by player {playerId}");

        // Check if all players have played
        CheckIfAllPlayersReady();
    }

    private void CheckIfAllPlayersReady()
    {
        if (!IsServer) return;

        // Get number of connected clients
        int connectedClients = (int)NetworkManager.Singleton.ConnectedClientsIds.Count;
        
        // Check if we have cards from all players (excluding host if host doesn't play)
        int expectedPlayers = connectedClients - 1; // Assuming host is shared screen, not a player
        
        if (playedCards.Count >= expectedPlayers && expectedPlayers > 0)
        {
            allPlayersReady.Value = true;
            Debug.Log("[GameScreenToGameBoardSync] All players ready!");
            NotifyGameBoardReadyClientRpc();
        }
    }

    [ClientRpc]
    private void NotifyGameBoardReadyClientRpc()
    {
        Debug.Log("[GameScreenToGameBoardSync] Notifying GameBoard that all cards are played");
        
        // If this is the shared screen (host), trigger GameBoard to display results
        if (IsServer)
        {
            TriggerGameBoardDisplay();
        }
    }

    private void TriggerGameBoardDisplay()
    {
        // Find GameBoard controller and tell it to display the played cards
        var boardController = FindObjectOfType<BoardController>();
        if (boardController != null)
        {
            Debug.Log("[GameScreenToGameBoardSync] Triggering GameBoard display");
            // BoardController will read from playedCards NetworkList
        }
        else
        {
            Debug.LogWarning("[GameScreenToGameBoardSync] BoardController not found in scene");
        }
    }

    private void OnPlayedCardsChanged(NetworkListEvent<CardPlayData> changeEvent)
    {
        Debug.Log($"[GameScreenToGameBoardSync] Played cards list changed. Count: {playedCards.Count}");
    }

    /// <summary>
    /// Get all played cards (for GameBoard to display)
    /// </summary>
    public List<CardPlayData> GetPlayedCards()
    {
        List<CardPlayData> cards = new List<CardPlayData>();
        foreach (var card in playedCards)
        {
            cards.Add(card);
        }
        return cards;
    }

    /// <summary>
    /// Clear played cards (for next round)
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ClearPlayedCardsServerRpc()
    {
        if (!IsServer) return;

        playedCards.Clear();
        allPlayersReady.Value = false;
        Debug.Log("[GameScreenToGameBoardSync] Played cards cleared");
    }

    public bool AreAllPlayersReady()
    {
        return allPlayersReady.Value;
    }
}

/// <summary>
/// Data structure for a played card
/// </summary>
public struct CardPlayData : INetworkSerializable, System.IEquatable<CardPlayData>
{
    public FixedString64Bytes cardId;
    public FixedString128Bytes cardTitle;
    public ulong playerId;
    public float timestamp;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref cardId);
        serializer.SerializeValue(ref cardTitle);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref timestamp);
    }

    public CardPlayData(string id, string title, ulong player, float time)
    {
        cardId = new FixedString64Bytes(id);
        cardTitle = new FixedString128Bytes(title);
        playerId = player;
        timestamp = time;
    }

    public bool Equals(CardPlayData other)
    {
        return cardId.Equals(other.cardId) && 
               cardTitle.Equals(other.cardTitle) && 
               playerId == other.playerId && 
               System.Math.Abs(timestamp - other.timestamp) < 0.001f;
    }

    public override bool Equals(object obj)
    {
        return obj is CardPlayData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(cardId, cardTitle, playerId, timestamp);
    }
}
