using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class VotingManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text resultsText;
    public TMP_Text winnerText;
    public RectTransform MainCardAnchor;     // card during voting
    public RectTransform ResultsCardAnchor;  // card for results UI
    public float cardFlyDuration = 0.5f;

    [Header("References")]
    public PlayedCardStore cardStorage; // assign in inspector
    public TimerBarUI votingTimer;      // assign in inspector


    private List<string> votes = new List<string>();
    private int yesCount = 0;
    private int noCount = 0;

    public string playerName = "Player1";

    // === Voting ===
    public void VoteYes()
    {
        votes.Add(playerName + " voted Yes");
        yesCount++;       // increment yes count
        UpdateResults();
    }

    public void VoteNo()
    {
        votes.Add(playerName + " voted No");
        noCount++;        // increment no count
        UpdateResults();
    }

    void UpdateResults()
    {
        resultsText.text = "Results:\n\n";
        foreach (string v in votes)
        {
            resultsText.text += v + "\n\n";
        }
    }

    public void ShowWinner()
    {
        if (yesCount > noCount)
            winnerText.text = "Winner: YES";
        else if (noCount > yesCount)
            winnerText.text = "Winner: NO";
        else
            winnerText.text = "Winner: TIE";

        // After showing winner, spawn the card in results
        SpawnCardInResults();
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnCardNextFrame());
    }

    private IEnumerator SpawnCardNextFrame()
    {
        // Wait a frame to ensure the UI is active and layout resolved
        yield return null;
        yield return new WaitForEndOfFrame();

        if (cardStorage == null || cardStorage.PlayedCardPrefab == null)
        {
            Debug.LogWarning("No card prefab in storage or storage not assigned!");
            yield break;
        }

        GameObject prefab = cardStorage.PlayedCardPrefab;
        Debug.Log("Spawning card prefab: " + prefab.name);

        // Instantiate prefab without parent first
        GameObject cardGO = Instantiate(prefab);

        // Make sure it's a UI element
        RectTransform card = cardGO.GetComponent<RectTransform>();
        if (card == null)
        {
            Debug.LogError("Card prefab must have a RectTransform!");
            yield break;
        }

        // Parent under MainCardAnchor
        card.SetParent(MainCardAnchor, false);
        card.localRotation = Quaternion.identity;

        // Wait one frame to allow layout to settle
        yield return null;

        // Animate from below and scale up
        Vector2 finalPos = card.anchoredPosition;
        card.anchoredPosition = new Vector2(finalPos.x, -900f);
        card.localScale = Vector3.one * 1.5f;

        float t = 0f;
        while (t < cardFlyDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / cardFlyDuration);
            card.anchoredPosition = Vector2.Lerp(new Vector2(finalPos.x, -900f), finalPos, p);
            yield return null;
        }

        // At the very end of SpawnCardNextFrame(), after setting card.anchoredPosition
        card.anchoredPosition = finalPos;

        // Start the timer now that the card is in place
        if (votingTimer != null)
            votingTimer.StartTimer();

        Debug.Log("Card prefab parented, animated under MainCardAnchor, and scaled up.");
    }

    private void SpawnCardInResults()
    {
        if (cardStorage == null || cardStorage.PlayedCardPrefab == null || ResultsCardAnchor == null)
            return;

        GameObject prefab = cardStorage.PlayedCardPrefab;
        GameObject cardGO = Instantiate(prefab, ResultsCardAnchor);
        RectTransform card = cardGO.GetComponent<RectTransform>();
        if (card != null)
        {
            card.localScale = Vector3.one * 2f;   // scale for results
            card.localRotation = Quaternion.identity;
            card.anchoredPosition = Vector2.zero; // center in anchor
        }

        Debug.Log("Card prefab spawned under ResultsCardAnchor.");
    }
}
