using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardLibraryUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<CardData> allCards = new List<CardData>();

    [Header("Available Cards UI (left)")]
    [SerializeField] private RectTransform availableContent;  // ScrollView_Available/Viewport/AvailableContent
    [SerializeField] private GameObject cardItemPrefab;       // CardItem prefab with CardItemView

    [Header("Chosen Cards UI (right)")]
    [SerializeField] private RectTransform chosenContent;     // ScrollView_Chosen/Viewport/ChosenContent
    [SerializeField] private TextMeshProUGUI projectsCountText;
    [SerializeField] private TextMeshProUGUI policiesCountText;
    [SerializeField] private TextMeshProUGUI shortTermCountText;

    // layout settings (you can tweak these in inspector if you want)
    [Header("Layout")]
    [SerializeField] private float verticalSpacing = 10f;
    [SerializeField] private float topPadding = 10f;

    private readonly List<CardData> chosenCards = new List<CardData>();

    private void Start()
    {
        if (allCards == null || allCards.Count == 0)
        {
            CreateDummyData();
        }

        RefreshAvailableList(allCards);
        RefreshChosenList();
    }

    private void CreateDummyData()
    {
        allCards = new List<CardData>
        {
            // Projects
            new CardData { id = "P1", title = "Train station",            category = CardCategory.Project },
            new CardData { id = "P2", title = "River dam upgrade",        category = CardCategory.Project },
            new CardData { id = "P3", title = "New water reservoir",      category = CardCategory.Project },
            new CardData { id = "P4", title = "Underground parking",      category = CardCategory.Project },
            new CardData { id = "P5", title = "Harbor expansion",         category = CardCategory.Project },
            new CardData { id = "P6", title = "City park redesign",       category = CardCategory.Project },
            new CardData { id = "P7", title = "New tram line",            category = CardCategory.Project },
            new CardData { id = "P8", title = "Flood barrier bridge",     category = CardCategory.Project },

            // Policies
            new CardData { id = "L1", title = "Trails",                   category = CardCategory.Policy },
            new CardData { id = "L2", title = "Water use tax",            category = CardCategory.Policy },
            new CardData { id = "L3", title = "Green roof subsidy",       category = CardCategory.Policy },
            new CardData { id = "L4", title = "Floodplain zoning",        category = CardCategory.Policy },
            new CardData { id = "L5", title = "Public transport pass",    category = CardCategory.Policy },
            new CardData { id = "L6", title = "Car-free city center",     category = CardCategory.Policy },
            new CardData { id = "L7", title = "Industrial emission cap",  category = CardCategory.Policy },
            new CardData { id = "L8", title = "Water quality regulation", category = CardCategory.Policy },

            // Short-term actions
            new CardData { id = "S1", title = "Wooden platform",          category = CardCategory.ShortTerm },
            new CardData { id = "S2", title = "Temporary pumps",          category = CardCategory.ShortTerm },
            new CardData { id = "S3", title = "Sandbag barrier",          category = CardCategory.ShortTerm },
            new CardData { id = "S4", title = "Evacuation drill",         category = CardCategory.ShortTerm },
            new CardData { id = "S5", title = "Water saving campaign",    category = CardCategory.ShortTerm },
            new CardData { id = "S6", title = "Parking restriction",      category = CardCategory.ShortTerm },
            new CardData { id = "S7", title = "River access closure",     category = CardCategory.ShortTerm },
            new CardData { id = "S8", title = "Emergency alert test",     category = CardCategory.ShortTerm }
        };
    }

    // =========================================================
    //  Available list (left)
    // =========================================================

    private void RefreshAvailableList(IEnumerable<CardData> cards)
    {
        if (availableContent == null || cardItemPrefab == null)
        {
            Debug.LogWarning("CardLibraryUI: AvailableContent or CardItemPrefab is not assigned.");
            return;
        }

        // Clear existing children
        for (int i = availableContent.childCount - 1; i >= 0; i--)
        {
            Destroy(availableContent.GetChild(i).gameObject);
        }

        // We need the height of a single card
        float cardHeight = GetCardHeight();
        int index = 0;

        foreach (var card in cards)
        {
            GameObject go = Instantiate(cardItemPrefab, availableContent);
            var view = go.GetComponent<CardItemView>();
            if (view == null)
            {
                Debug.LogError("CardLibraryUI: CardItemPrefab is missing CardItemView component.");
                continue;
            }

            view.Init(card, this, isChosenList: false);

            // Manually position the card under the previous one
            var rt = go.GetComponent<RectTransform>();
            LayoutCard(rt, availableContent, index, cardHeight);
            index++;
        }

        // Set the content height so scrolling works
        SetContentHeight(availableContent, index, cardHeight);
    }

    // =========================================================
    //  Chosen list (right)
    // =========================================================

    private void RefreshChosenList()
    {
        if (chosenContent == null || cardItemPrefab == null)
        {
            Debug.LogWarning("CardLibraryUI: ChosenContent or CardItemPrefab is not assigned.");
            return;
        }

        for (int i = chosenContent.childCount - 1; i >= 0; i--)
        {
            Destroy(chosenContent.GetChild(i).gameObject);
        }

        float cardHeight = GetCardHeight();
        int index = 0;

        foreach (var card in chosenCards)
        {
            GameObject go = Instantiate(cardItemPrefab, chosenContent);
            var view = go.GetComponent<CardItemView>();
            if (view == null)
            {
                Debug.LogError("CardLibraryUI: CardItemPrefab is missing CardItemView component.");
                continue;
            }

            view.Init(card, this, isChosenList: true);

            var rt = go.GetComponent<RectTransform>();
            LayoutCard(rt, chosenContent, index, cardHeight);
            index++;
        }

        SetContentHeight(chosenContent, index, cardHeight);
        UpdateCounters();
    }

    // =========================================================
    //  Layout helpers (no LayoutGroups used)
    // =========================================================

    private float GetCardHeight()
    {
        var rt = cardItemPrefab.GetComponent<RectTransform>();
        return rt != null ? rt.rect.height : 100f;
    }

    private void LayoutCard(RectTransform cardRT, RectTransform parentContent, int index, float cardHeight)
    {
        // Stretch horizontally, stick to top
        cardRT.anchorMin = new Vector2(0f, 1f);
        cardRT.anchorMax = new Vector2(1f, 1f);
        cardRT.pivot     = new Vector2(0.5f, 1f);

        // Left/right padding inside content
        float leftPadding  = 10f;
        float rightPadding = 10f;

        cardRT.offsetMin = new Vector2(leftPadding, 0f);   // left
        cardRT.offsetMax = new Vector2(-rightPadding, 0f); // right
        cardRT.sizeDelta = new Vector2(cardRT.sizeDelta.x, cardHeight);

        float y = -(topPadding + index * (cardHeight + verticalSpacing));
        cardRT.anchoredPosition = new Vector2(0f, y);
    }

    private void SetContentHeight(RectTransform content, int count, float cardHeight)
    {
        float height = topPadding + count * (cardHeight + verticalSpacing);
        if (height < 0) height = 0;

        Vector2 size = content.sizeDelta;
        size.y = height;
        content.sizeDelta = size;
    }

    // =========================================================
    //  Click handlers (called from CardItemView)
    // =========================================================

    public void OnCardClickedFromAvailable(CardData data)
    {
        if (data == null) return;

        if (!chosenCards.Any(c => c.id == data.id))
        {
            chosenCards.Add(data);
            RefreshChosenList();
        }
    }

    public void OnCardClickedFromChosen(CardData data)
    {
        if (data == null) return;

        if (chosenCards.RemoveAll(c => c.id == data.id) > 0)
        {
            RefreshChosenList();
        }
    }

    // =========================================================
    //  Counters
    // =========================================================

    private void UpdateCounters()
    {
        int projects  = chosenCards.Count(c => c.category == CardCategory.Project);
        int policies  = chosenCards.Count(c => c.category == CardCategory.Policy);
        int shortTerm = chosenCards.Count(c => c.category == CardCategory.ShortTerm);

        if (projectsCountText != null)
            projectsCountText.text = $"Projects {projects}/20";

        if (policiesCountText != null)
            policiesCountText.text = $"Policies {policies}/40";

        if (shortTermCountText != null)
            shortTermCountText.text = $"Short-term {shortTerm}/40";
    }

    // =========================================================
    //  Filter buttons (tabs)
    // =========================================================

    public void ShowAll()
    {
        RefreshAvailableList(allCards);
    }

    public void ShowPolicies()
    {
        RefreshAvailableList(allCards.Where(c => c.category == CardCategory.Policy));
    }

    public void ShowProjects()
    {
        RefreshAvailableList(allCards.Where(c => c.category == CardCategory.Project));
    }

    public void ShowShortTerm()
    {
        RefreshAvailableList(allCards.Where(c => c.category == CardCategory.ShortTerm));
    }

    // Optionally override dummy data
    public void SetCards(List<CardData> cards)
    {
        allCards = cards ?? new List<CardData>();
        RefreshAvailableList(allCards);
        chosenCards.Clear();
        RefreshChosenList();
    }
}
