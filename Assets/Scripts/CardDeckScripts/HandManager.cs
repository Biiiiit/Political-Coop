using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    [Header("Hand Movement Settings")]
    public float moveUpAmount = 60f;
    public float moveDuration = 0.4f;

    [Header("Card Zoom Settings")]
    public float zoomDuration = 0.3f;
    public float zoomScale = 2f;

    private RectTransform handArea;
    private Vector2 handStartPos;
    private bool hasMoved = false;

    private LayoutGroup handLayout;

    private List<RectTransform> cardsInHand = new List<RectTransform>();
    private Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();

    // Zoom tracking
    private bool isCardZoomed = false;
    private RectTransform zoomedCard = null;
    private Vector2 zoomedOriginalPos;
    private Vector3 zoomedOriginalScale;

    private void Awake()
    {
        handArea = GetComponent<RectTransform>();
        handStartPos = handArea.anchoredPosition;

        handLayout = handArea.GetComponent<LayoutGroup>();
    }

    // Move the hand panel up
    public void MoveHandUp()
    {
        if (hasMoved) return;
        hasMoved = true;
        StartCoroutine(MoveUpRoutine());
    }

    private IEnumerator MoveUpRoutine()
    {
        Vector2 targetPos = handStartPos + Vector2.up * moveUpAmount;
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = 1f - Mathf.Pow(1f - t / moveDuration, 3f);
            handArea.anchoredPosition = Vector2.Lerp(handStartPos, targetPos, p);
            yield return null;
        }
        handArea.anchoredPosition = targetPos;
    }

    // Register a card
    public void RegisterCard(RectTransform card)
    {
        cardsInHand.Add(card);
        originalPositions[card] = card.anchoredPosition;

        Button btn = card.gameObject.GetComponent<Button>();
        if (btn == null) btn = card.gameObject.AddComponent<Button>();

        btn.onClick.AddListener(() => OnCardClicked(card));
    }

    private void OnCardClicked(RectTransform card)
    {
        if (!isCardZoomed)
        {
            // Zoom in
            isCardZoomed = true;
            zoomedCard = card;

            zoomedOriginalPos = card.anchoredPosition;
            zoomedOriginalScale = card.localScale;

            // Disable LayoutGroup temporarily so hand cards don't interfere
            if (handLayout != null)
                handLayout.enabled = false;

            // Move card to top
            card.SetAsLastSibling();

            // Disable other cards
            foreach (RectTransform c in cardsInHand)
            {
                if (c != card)
                    c.GetComponent<Button>().interactable = false;
            }

            // Zoom target position
            Vector2 targetPos = new Vector2(850f, -200f); // adjust offset as needed
            StartCoroutine(ZoomCardCoroutine(card, targetPos, zoomedOriginalScale * zoomScale));
        }
        else if (zoomedCard == card)
        {
            // Zoom out
            StartCoroutine(ZoomOutRoutine(card));
        }
    }

    private IEnumerator ZoomCardCoroutine(RectTransform card, Vector2 targetPos, Vector3 targetScale)
    {
        Vector2 startPos = card.anchoredPosition;
        Vector3 startScale = card.localScale;
        float t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float p = t / zoomDuration;
            card.anchoredPosition = Vector2.Lerp(startPos, targetPos, p);
            card.localScale = Vector3.Lerp(startScale, targetScale, p);
            yield return null;
        }

        card.anchoredPosition = targetPos;
        card.localScale = targetScale;
    }

    private IEnumerator ZoomOutRoutine(RectTransform card)
    {
        Vector2 startPos = card.anchoredPosition;
        Vector3 startScale = card.localScale;
        float t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float p = t / zoomDuration;
            card.anchoredPosition = Vector2.Lerp(startPos, zoomedOriginalPos, p);
            card.localScale = Vector3.Lerp(startScale, zoomedOriginalScale, p);
            yield return null;
        }

        card.anchoredPosition = zoomedOriginalPos;
        card.localScale = zoomedOriginalScale;

        // Reset other cards to their original positions
        foreach (var c in cardsInHand)
        {
            if (c != card)
                c.anchoredPosition = originalPositions[c];
        }

        // Re-enable other cards
        foreach (RectTransform c in cardsInHand)
        {
            if (c != card)
                c.GetComponent<Button>().interactable = true;
        }

        // Re-enable LayoutGroup
        if (handLayout != null)
            handLayout.enabled = true;

        isCardZoomed = false;
        zoomedCard = null;
    }
}
