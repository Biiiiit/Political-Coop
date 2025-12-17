using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Decks")]
    public List<GameObject> deck1 = new List<GameObject>();
    public List<GameObject> deckBacks1 = new List<GameObject>();
    private List<GameObject> deckBackInstances1 = new List<GameObject>();
    private Vector3 basePos1 = new Vector3(-12f, 6f, 10f);

    public List<GameObject> deck2 = new List<GameObject>();
    public List<GameObject> deckBacks2 = new List<GameObject>();
    private List<GameObject> deckBackInstances2 = new List<GameObject>();
    private Vector3 basePos2 = new Vector3(0f, 6f, 10f);

    public List<GameObject> deck3 = new List<GameObject>();
    public List<GameObject> deckBacks3 = new List<GameObject>();
    private List<GameObject> deckBackInstances3 = new List<GameObject>();
    private Vector3 basePos3 = new Vector3(12f, 6f, 10f);

    [Header("References")]
    public RectTransform handArea;
    public HandManager handManager;   // ← ADDED

    [Header("Draw Settings")]
    public float drawDelay = 0.15f;

    [Header("Animation")]
    public float fallDuration = 0.25f;
    public float handDuration = 0.35f;
    public float startOffset = 500f;
    public float fallDistanceY = 20f;
    public float fallDistanceZ = 5f;
    public float stackOffsetY = 0.1f;

    private void Start()
    {
        CreateBackStack(deckBacks1, deckBackInstances1, basePos1);
        CreateBackStack(deckBacks2, deckBackInstances2, basePos2);
        CreateBackStack(deckBacks3, deckBackInstances3, basePos3);
    }

    private void CreateBackStack(List<GameObject> backs, List<GameObject> backInstances, Vector3 basePos)
    {
        foreach (var back in backInstances)
            if (back != null) Destroy(back);
        backInstances.Clear();

        int count = backs.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject backInstance = Instantiate(backs[i], this.transform);
            float offsetY = stackOffsetY * (count - i - 1);
            backInstance.transform.localPosition = basePos + new Vector3(0, offsetY, 0);
            backInstance.transform.localRotation = Quaternion.Euler(21f, 0f, 0f);
            backInstances.Add(backInstance);
        }
    }

    // ------------------------------
    // Draw sequence with delays between deck animations
    // ------------------------------
    public void DrawCustomSequence()
    {
        StartCoroutine(DrawSequenceWithDelay());
    }

    private IEnumerator DrawSequenceWithDelay()
    {
        List<GameObject> prefabsToDraw = new List<GameObject>();

        // 2 cards from deck1
        for (int i = 0; i < 2; i++)
        {
            if (deck1.Count > 0 && deckBackInstances1.Count > 0)
            {
                GameObject cardPrefab = deck1[0];
                deck1.RemoveAt(0);
                prefabsToDraw.Add(cardPrefab);

                GameObject back = deckBackInstances1[deckBackInstances1.Count - 1];
                deckBackInstances1.RemoveAt(deckBackInstances1.Count - 1);

                yield return AnimateBack(back);
                yield return new WaitForSeconds(drawDelay);
            }
        }

        // 1 card from deck2
        if (deck2.Count > 0 && deckBackInstances2.Count > 0)
        {
            GameObject cardPrefab = deck2[0];
            deck2.RemoveAt(0);
            prefabsToDraw.Add(cardPrefab);

            GameObject back = deckBackInstances2[deckBackInstances2.Count - 1];
            deckBackInstances2.RemoveAt(deckBackInstances2.Count - 1);

            yield return AnimateBack(back);
            yield return new WaitForSeconds(drawDelay);
        }

        // 1 card from deck3
        if (deck3.Count > 0 && deckBackInstances3.Count > 0)
        {
            GameObject cardPrefab = deck3[0];
            deck3.RemoveAt(0);
            prefabsToDraw.Add(cardPrefab);

            GameObject back = deckBackInstances3[deckBackInstances3.Count - 1];
            deckBackInstances3.RemoveAt(deckBackInstances3.Count - 1);

            yield return AnimateBack(back);
            yield return new WaitForSeconds(drawDelay);
        }

        // Animate all card prefabs into hand sequentially
        foreach (var prefab in prefabsToDraw)
            yield return AnimateCardPrefab(prefab);

        // ← MOVE HAND AFTER ALL CARDS ARE DEALT
        handManager.MoveHandUp();
    }

    private IEnumerator AnimateBack(GameObject backInstance)
    {
        Vector3 startPos = backInstance.transform.position;

        Vector3 slideDir =
            backInstance.transform.up * -fallDistanceY +
            backInstance.transform.forward * -fallDistanceZ;

        Vector3 endPos = startPos + slideDir;

        float t = 0f;
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float p = 1f - Mathf.Pow(1f - t / fallDuration, 3f);
            backInstance.transform.position = Vector3.Lerp(startPos, endPos, p);
            yield return null;
        }

        Destroy(backInstance);
    }

    private IEnumerator AnimateCardPrefab(GameObject prefab)
    {
        GameObject card = Instantiate(prefab, handArea);
        card.transform.SetParent(handArea, false);

        RectTransform rt = card.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Card prefab must have a RectTransform!");
            yield break;
        }

        rt.sizeDelta = new Vector2(280, 449);

        CanvasGroup cg = card.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        yield return null;

        Vector2 finalPos = rt.anchoredPosition;
        Vector2 startPos = finalPos + new Vector2(0, -startOffset);
        rt.anchoredPosition = startPos;
        cg.alpha = 1f;

        float t = 0f;
        while (t < handDuration)
        {
            t += Time.deltaTime;
            float p = 1f - Mathf.Pow(1f - t / handDuration, 3f);
            rt.anchoredPosition = Vector2.Lerp(startPos, finalPos, p);
            yield return null;
        }

        rt.anchoredPosition = finalPos;

        // Add this after creating the card prefab in DeckManager
        RectTransform cardRect = card.GetComponent<RectTransform>();
        handManager.RegisterCard(cardRect);

    }
}
