using UnityEngine;
using System.Collections;

public class CardResultsHandler : MonoBehaviour
{
    public RectTransform cardAnchor;
    public float startDelay = 0.5f;      // ⏱ delay before spin starts
    public float spinSpeed = 720f;
    public float shrinkDuration = 0.6f;

    public void HandleCard(GameObject card)
    {
        RectTransform rt = card.GetComponent<RectTransform>();

        rt.SetParent(cardAnchor, false);
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        StartCoroutine(SpinAndConsume(card));
    }

    private IEnumerator SpinAndConsume(GameObject card)
    {
        // 🔹 Delay before animation starts
        yield return new WaitForSeconds(startDelay);

        RectTransform rt = card.GetComponent<RectTransform>();
        Vector3 startScale = rt.localScale;
        float t = 0f;

        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float p = t / shrinkDuration;

            // Spin
            rt.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

            // Shrink
            rt.localScale = Vector3.Lerp(startScale, Vector3.zero, p);

            yield return null;
        }

        rt.localScale = Vector3.zero;

        ActivateCard();

        // Destroy the card
        Destroy(card);

        // ✅ Disable this CardResultsHandler GameObject
        gameObject.SetActive(false);
    }

    private void ActivateCard()
    {
        // Placeholder logic
        Debug.Log("ActivateCard() called");
    }
}
