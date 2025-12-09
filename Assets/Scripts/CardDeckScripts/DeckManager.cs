using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<GameObject> cardPrefabs;

    public RectTransform deckPosition;
    public RectTransform handArea;

    public float duration = 0.4f;

    public void DrawCard()
    {
        if (cardPrefabs.Count == 0) return;

        GameObject prefab = cardPrefabs[0];
        cardPrefabs.RemoveAt(0);

        // Spawn as child of handArea immediately
        GameObject card = Instantiate(prefab, handArea);

        RectTransform cardRT = card.GetComponent<RectTransform>();
        cardRT.sizeDelta = new Vector2(280, 449); // tweak to fit
        cardRT.localScale = Vector3.zero;         // start small

        // Animate the whole prefab
        StartCoroutine(AnimateCardScale(cardRT));
    }

    private IEnumerator AnimateCardScale(RectTransform cardRT)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / duration);
            cardRT.localScale = Vector3.Lerp(startScale, endScale, p);
            yield return null;
        }

        cardRT.localScale = endScale;
    }
}
