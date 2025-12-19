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

    [Header("Play Card Settings")]
    public float playFlyDistance = 600f;
    public float playFlyDuration = 0.4f;

    [Header("Selected Card Button")]
    public RectTransform cardActionButton;

    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private RectTransform deckCanvas;   // Assign DeckCanvas RectTransform

    private RectTransform handArea;
    private Vector2 handStartPos;
    private bool hasMoved = false;

    private LayoutGroup handLayout;

    private List<RectTransform> cardsInHand = new List<RectTransform>();
    private Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();

    // Zoom tracking
    private bool isCardZoomed = false;
    private RectTransform zoomedCard = null;
    private Vector3 zoomedOriginalWorldPos;
    private Vector3 zoomedOriginalScale;
    private Transform zoomedOriginalParent;

    // Dark background
    private Image dimBackground;

    private void Awake()
    {
        handArea = GetComponent<RectTransform>();
        handStartPos = handArea.anchoredPosition;

        handLayout = handArea.GetComponent<LayoutGroup>();

        if (cardActionButton != null)
            cardActionButton.gameObject.SetActive(false);
    }

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

    public void RegisterCard(RectTransform card)
    {
        cardsInHand.Add(card);
        originalPositions[card] = card.anchoredPosition;

        Button btn = card.GetComponent<Button>();
        if (btn == null)
            btn = card.gameObject.AddComponent<Button>();

        btn.onClick.AddListener(() => OnCardClicked(card));
    }

    private void OnCardClicked(RectTransform card)
    {
        if (!isCardZoomed)
        {
            isCardZoomed = true;
            zoomedCard = card;

            zoomedOriginalParent = card.parent;
            zoomedOriginalScale = card.localScale;
            zoomedOriginalWorldPos = card.position;

            // Reparent to deckCanvas and keep world position
            card.SetParent(deckCanvas, true);
            card.position = zoomedOriginalWorldPos;
            card.SetAsLastSibling();

            if (handLayout != null)
                handLayout.enabled = false;

            foreach (RectTransform c in cardsInHand)
            {
                if (c != card)
                    c.GetComponent<Button>().interactable = false;
            }

            // Create dim background behind card
            CreateDimBackground();

            // Animate card to center
            Vector3 targetWorldPos = deckCanvas.TransformPoint(((RectTransform)deckCanvas).rect.center);
            StartCoroutine(ZoomCardCoroutine(card, targetWorldPos, zoomedOriginalScale * zoomScale, true)); // true = show button after zoom
        }
        else if (zoomedCard == card)
        {
            StartCoroutine(ZoomOutRoutine(card));
        }
    }

    private void CreateDimBackground()
    {
        if (dimBackground != null) return; // Already exists

        GameObject bgGO = new GameObject("DimBackground", typeof(RectTransform));
        bgGO.transform.SetParent(deckCanvas, false);

        RectTransform rt = bgGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        dimBackground = bgGO.AddComponent<Image>();
        dimBackground.color = new Color(0, 0, 0, 0); // Start transparent

        // Place background **just below the zoomed card** in hierarchy
        if (zoomedCard != null)
            bgGO.transform.SetSiblingIndex(zoomedCard.GetSiblingIndex());

        StartCoroutine(FadeBackground(0f, 0.5f, zoomDuration));
    }

    private IEnumerator FadeBackground(float fromAlpha, float toAlpha, float duration)
    {
        float t = 0f;
        Color c = dimBackground.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            c.a = Mathf.Lerp(fromAlpha, toAlpha, p);
            dimBackground.color = c;
            yield return null;
        }

        c.a = toAlpha;
        dimBackground.color = c;
    }

    private IEnumerator ZoomCardCoroutine(RectTransform card, Vector3 targetWorldPos, Vector3 targetScale, bool showButtonAfter = false)
    {
        Vector3 startPos = card.position;
        Vector3 startScale = card.localScale;
        float t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float p = t / zoomDuration;
            card.position = Vector3.Lerp(startPos, targetWorldPos, p);
            card.localScale = Vector3.Lerp(startScale, targetScale, p);
            yield return null;
        }

        card.position = targetWorldPos;
        card.localScale = targetScale;

        if (showButtonAfter)
            ShowCardButton();
    }

    private IEnumerator ZoomOutRoutine(RectTransform card)
    {
        // Hide the card button immediately
        HideCardButton();

        Vector3 startPos = card.position;
        Vector3 startScale = card.localScale;
        float t = 0f;

        // Fade background back
        if (dimBackground != null)
            StartCoroutine(FadeBackground(0.5f, 0f, zoomDuration));

        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float p = t / zoomDuration;
            card.position = Vector3.Lerp(startPos, zoomedOriginalWorldPos, p);
            card.localScale = Vector3.Lerp(startScale, zoomedOriginalScale, p);
            yield return null;
        }

        card.position = zoomedOriginalWorldPos;
        card.localScale = zoomedOriginalScale;

        // Reparent back to hand
        card.SetParent(zoomedOriginalParent, true);

        // Remove background
        if (dimBackground != null)
        {
            Destroy(dimBackground.gameObject);
            dimBackground = null;
        }

        ResetHandState();
    }

    public void PlaySelectedCard()
    {
        if (!isCardZoomed || zoomedCard == null) return;

        GameObject prefab = deckManager.GetPrefabForCard(zoomedCard);
        if (prefab != null)
        {
            PlayedCardStore.Instance.StoreCard(prefab);
        }

        HideCardButton();
        StartCoroutine(PlayCardRoutine(zoomedCard));
    }

    [SerializeField] private GameObject deckCanvasGO;
    [SerializeField] private GameObject discussionUI;
    [SerializeField] public VotingManager VotingManagerInstance;

    private IEnumerator PlayCardRoutine(RectTransform card)
    {
        Vector2 startPos = card.anchoredPosition;
        Vector2 targetPos = startPos + Vector2.up * playFlyDistance;
        float t = 0f;

        while (t < playFlyDuration)
        {
            t += Time.deltaTime;
            float p = t / playFlyDuration;
            card.anchoredPosition = Vector2.Lerp(startPos, targetPos, p);
            yield return null;
        }

        card.gameObject.SetActive(false);
        cardsInHand.Remove(card);
        originalPositions.Remove(card);

        if (deckCanvasGO != null) deckCanvasGO.SetActive(false);
        if (discussionUI != null) discussionUI.SetActive(true);
        if (VotingManagerInstance != null) VotingManagerInstance.gameObject.SetActive(true);

        ResetHandState();
    }

    private void ResetHandState()
    {
        foreach (RectTransform c in cardsInHand)
            c.GetComponent<Button>().interactable = true;

        if (handLayout != null)
            handLayout.enabled = true;

        isCardZoomed = false;
        zoomedCard = null;
    }

    private void ShowCardButton()
    {
        if (cardActionButton == null) return;
        cardActionButton.gameObject.SetActive(true);
        cardActionButton.SetAsLastSibling();
    }

    private void HideCardButton()
    {
        if (cardActionButton == null) return;
        cardActionButton.gameObject.SetActive(false);
    }
}
