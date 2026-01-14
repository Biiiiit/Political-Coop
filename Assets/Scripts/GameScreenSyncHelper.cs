using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Helper component for GameScreen to integrate with the sync system.
/// Attach this to GameScreen scene to enable syncing with GameBoard.
/// </summary>
public class GameScreenSyncHelper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameScreen gameScreen;

    private void Start()
    {
        if (gameScreen == null)
        {
            gameScreen = FindObjectOfType<GameScreen>();
        }

        if (gameScreen == null)
        {
            Debug.LogWarning("[GameScreenSyncHelper] GameScreen not found in scene");
        }
    }

    /// <summary>
    /// Call this when a player plays a card
    /// </summary>
    public void OnCardPlayed(string cardId, string cardTitle)
    {
        if (GameScreenToGameBoardSync.Instance == null)
        {
            Debug.LogWarning("[GameScreenSyncHelper] GameScreenToGameBoardSync.Instance not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("[GameScreenSyncHelper] Not connected to network");
            return;
        }

        ulong playerId = NetworkManager.Singleton.LocalClientId;
        GameScreenToGameBoardSync.Instance.ReportCardPlayed(cardId, cardTitle, playerId);

        Debug.Log($"[GameScreenSyncHelper] Reported card played: {cardTitle}");
    }

    /// <summary>
    /// Check if all players are ready (all cards played)
    /// </summary>
    public bool AreAllPlayersReady()
    {
        if (GameScreenToGameBoardSync.Instance == null)
            return false;

        return GameScreenToGameBoardSync.Instance.AreAllPlayersReady();
    }
}
