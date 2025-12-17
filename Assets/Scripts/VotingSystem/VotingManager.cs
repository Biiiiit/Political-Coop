using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class VotingManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text resultsText;
    public TMP_Text winnerText;
    public Transform MainCardAnchor; // Where the card should end up
    public float cardFlyDuration = 0.5f;

    private List<string> votes = new List<string>();
    private int yesCount = 0;
    private int noCount = 0;

    public string playerName = "Player1";

    private RectTransform cardToAnimate = null;

    // === Voting ===
    public void VoteYes()
    {
        votes.Add(playerName + " voted Yes");
        UpdateResults();
    }

    public void VoteNo()
    {
        votes.Add(playerName + " voted No");
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
    }

    // === Receive card reference ===
    public void ReceiveCard(RectTransform card)
    {
        cardToAnimate = card;
    }

    // === Animate card when UI is enabled ===
    private void OnEnable()
    {
        if (cardToAnimate != null)
        {
            StartCoroutine(AnimateCardToAnchor(cardToAnimate));
        }
    }

    private IEnumerator AnimateCardToAnchor(RectTransform card)
    {
        // Keep card as a child of canvas
        Canvas canvas = MainCardAnchor.GetComponentInParent<Canvas>();
        card.SetParent(canvas.transform, worldPositionStays: true);

        // Start at bottom of screen
        Vector3 startPos = new Vector3(0, -canvas.pixelRect.height / 2f - card.rect.height, 0);
        card.position = startPos;

        // End position: MainCardAnchor
        Vector3 endPos = MainCardAnchor.position;

        float t = 0f;
        Vector3 startScale = card.localScale;
        Vector3 targetScale = card.localScale;

        while (t < cardFlyDuration)
        {
            t += Time.deltaTime;
            float p = t / cardFlyDuration;
            card.position = Vector3.Lerp(startPos, endPos, p);
            card.localScale = Vector3.Lerp(startScale, targetScale, p);
            yield return null;
        }

        card.position = endPos;
        card.SetParent(MainCardAnchor, worldPositionStays: false);
    }
}
