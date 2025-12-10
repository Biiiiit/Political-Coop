using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Deck")]
    public List<GameObject> deck = new List<GameObject>();

    [Header("References")]
    public RectTransform handArea;

    [Header("Animation")]
    public float duration = 0.35f;
    public float startOffset = 500f;

    // --------------------------------------------------
    // PUBLIC API
    // --------------------------------------------------

    // Replace the entire deck with a new list
    public void SetDeck(List<GameObject> newDeck)
    {
        deck = new List<GameObject>(newDeck);
    }

    // Add a single card to the deck
    public void AddCard(GameObject cardPrefab)
    {
        deck.Add(cardPrefab);
    }

    // Add multiple cards to the deck
    public void AddCards(List<GameObject> cards)
    {
        deck.AddRange(cards);
    }

    // Shuffle the deck
    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            GameObject temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    // Draw one card
    public void DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.Log("Deck is empty!");
            return;
        }

        GameObject prefab = deck[0];
        deck.RemoveAt(0);

        // Instantiate card in hand area
        GameObject card = Instantiate(prefab, handArea);
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(280, 449);

        // Hide card before layout snaps it
        CanvasGroup cg = card.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        StartCoroutine(AnimateCardIn(rt, cg));
    }

    // --------------------------------------------------
    // ANIMATION
    // --------------------------------------------------

    private IEnumerator AnimateCardIn(RectTransform rt, CanvasGroup cg)
    {
        // wait 1 frame for layout group to place the card
        yield return null;

        Vector2 finalPos = rt.anchoredPosition;

        Vector2 startPos = finalPos + new Vector2(0, -startOffset);
        rt.anchoredPosition = startPos;

        cg.alpha = 1f; // reveal AFTER offset is applied

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            // ease-out cubic
            p = 1f - Mathf.Pow(1f - p, 3f);

            rt.anchoredPosition = Vector2.Lerp(startPos, finalPos, p);
            yield return null;
        }

        rt.anchoredPosition = finalPos;
    }
}
