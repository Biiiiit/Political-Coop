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
    public PlayedCardStore cardStorage;
    public TimerBarUI votingTimer;
    private GameObject activeCard;           // card in discussion

    [Header("Handoff")]
    public CardResultsHandler resultsHandler;

    [Header("Fade")]
    public CanvasGroup resultsCanvasGroup;   // fade ResultsUI
    public float fadeDuration = 0.4f;

    private List<string> votes = new List<string>();
    private int yesCount = 0;
    private int noCount = 0;

    public string playerName = "Player1";

    // === Voting ===
    public void VoteYes()
    {
        votes.Add(playerName + " voted Yes");
        yesCount++;
        UpdateResults();
    }

    public void VoteNo()
    {
        votes.Add(playerName + " voted No");
        noCount++;
        UpdateResults();
    }

    void UpdateResults()
    {
        resultsText.text = "Results:\n\n";
        foreach (string v in votes)
            resultsText.text += v + "\n\n";
    }

    public void ShowWinner()
    {
        if (yesCount > noCount)
            winnerText.text = "Winner: YES";
        else if (noCount > yesCount)
            winnerText.text = "Winner: NO";
        else
            winnerText.text = "Winner: TIE";

        // Move card to ResultsUI immediately after voting
        MoveCardToResultsUI();
        // Start the sequence: wait 2s, handover, fade
        StartCoroutine(EndVotingSequence());
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnCardNextFrame());
    }

    private IEnumerator SpawnCardNextFrame()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        if (cardStorage == null || cardStorage.PlayedCardPrefab == null)
        {
            Debug.LogWarning("No card prefab in storage or storage not assigned!");
            yield break;
        }

        GameObject prefab = cardStorage.PlayedCardPrefab;
        GameObject cardGO = Instantiate(prefab);

        RectTransform card = cardGO.GetComponent<RectTransform>();
        if (card == null)
        {
            Debug.LogError("Card prefab must have a RectTransform!");
            yield break;
        }

        // Parent under MainCardAnchor (DiscussionUI)
        card.SetParent(MainCardAnchor, false);
        card.localRotation = Quaternion.identity;

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

        card.anchoredPosition = finalPos;

        if (votingTimer != null)
            votingTimer.StartTimer();

        activeCard = cardGO; // keep reference
        Debug.Log("Card spawned under MainCardAnchor (DiscussionUI).");
    }

    private void MoveCardToResultsUI()
    {
        if (activeCard != null && ResultsCardAnchor != null)
        {
            RectTransform rt = activeCard.GetComponent<RectTransform>();
            rt.SetParent(ResultsCardAnchor, false);
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one * 2f; // adjust size for ResultsUI

            Debug.Log("Card moved to ResultsUI (ResultsCardAnchor).");
        }
    }

    private IEnumerator EndVotingSequence()
    {
        // 1️⃣ Wait 2 seconds so card is visible in ResultsUI
        yield return new WaitForSeconds(2f);

        // 2️⃣ Hand over the card to CardResultsHandler
        if (activeCard != null && resultsHandler != null)
        {
            resultsHandler.HandleCard(activeCard);
            activeCard = null;
        }

        // 3️⃣ Fade out ResultsUI
        if (resultsCanvasGroup != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                resultsCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }

            resultsCanvasGroup.alpha = 0f;
            resultsCanvasGroup.interactable = false;
            resultsCanvasGroup.blocksRaycasts = false;

            // ✅ Disable the entire ResultsUI GameObject
            resultsCanvasGroup.gameObject.SetActive(false);
        }

        Debug.Log("ResultsUI faded out and disabled.");
    }
}
