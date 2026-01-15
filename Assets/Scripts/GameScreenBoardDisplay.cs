using UnityEngine;

public class GameScreenBoardDisplay : MonoBehaviour
{
    void Start()
    {
        // Only run on shared screen (host)
        if (DualScreenManager.Instance != null && DualScreenManager.Instance.IsSharedMode())
        {
            StartCoroutine(DisplayCardsAfterDelay());
        }
    }

    System.Collections.IEnumerator DisplayCardsAfterDelay()
    {
        // Wait for sync to be ready
        yield return new WaitForSeconds(0.5f);
        DisplayPlayedCards();
    }

    private void DisplayPlayedCards()
    {
        if (GameScreenToGameBoardSync.Instance == null)
        {
            Debug.LogWarning("GameScreenToGameBoardSync not found");
            return;
        }

        var playedCards = GameScreenToGameBoardSync.Instance.GetPlayedCards();
        
        foreach (var cardData in playedCards)
        {
            Debug.Log($"Player {cardData.playerId} played: {cardData.cardTitle}");
            // TODO: Display card on board UI
            // Example: InstantiateCardUI(cardData);
        }
    }
}