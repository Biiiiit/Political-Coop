using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardLibraryUI : MonoBehaviour
{
    // Which tab is active
    private enum FilterMode
    {
        All,
        Policy,
        Project,
        ShortTerm
    }

    [Header("Data")]
    [SerializeField] private List<CardData> allCards = new List<CardData>();

    [Header("Available Cards UI (left)")]
    [SerializeField] private RectTransform availableContent;      // ScrollView_Available/Viewport/AvailableContent
    [SerializeField] private GameObject cardItemPrefab;           // CardItem prefab with CardItemView

    [Header("Chosen Cards UI (right)")]
    [SerializeField] private RectTransform chosenContent;         // ScrollView_Chosen/Viewport/ChosenContent
    [SerializeField] private TextMeshProUGUI projectsCountText;
    [SerializeField] private TextMeshProUGUI policiesCountText;
    [SerializeField] private TextMeshProUGUI shortTermCountText;

    [Header("Grid layout (LEFT)")]
    [SerializeField] private int gridColumns = 3;
    [SerializeField] private float gridHorizontalSpacing = 10f;
    [SerializeField] private float gridVerticalSpacing = 10f;
    [SerializeField] private float gridTopPadding = 10f;
    [SerializeField] private float gridLeftPadding = 10f;

    [Header("List layout (RIGHT)")]
    [SerializeField] private float listVerticalSpacing = 10f;
    [SerializeField] private float listTopPadding = 10f;

    // Internal state
    private readonly List<CardData> chosenCards = new List<CardData>();
    private readonly HashSet<string> chosenIds = new HashSet<string>();
    private FilterMode currentFilter = FilterMode.All;

    private void Start()
    {
        if (allCards == null || allCards.Count == 0)
        {
            CreateDummyData();
        }

        RefreshAvailableList();
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
    //  AVAILABLE LIST (LEFT) – GRID LAYOUT
    // =========================================================

    private void RefreshAvailableList()
    {
        if (availableContent == null || cardItemPrefab == null)
        {
            Debug.LogWarning("CardLibraryUI: AvailableContent or CardItemPrefab is not assigned.");
            return;
        }

        // Determine which cards we show, based on filter and which are NOT chosen
        IEnumerable<CardData> cards = allCards;

        switch (currentFilter)
        {
            case FilterMode.Policy:
                cards = cards.Where(c => c.category == CardCategory.Policy);
                break;
            case FilterMode.Project:
                cards = cards.Where(c => c.category == CardCategory.Project);
                break;
            case FilterMode.ShortTerm:
                cards = cards.Where(c => c.category == CardCategory.ShortTerm);
                break;
            case FilterMode.All:
            default:
                break;
        }

        cards = cards.Where(c => !chosenIds.Contains(c.id));

        // Clear existing children
        for (int i = availableContent.childCount - 1; i >= 0; i--)
            Destroy(availableContent.GetChild(i).gameObject);

        float cardWidth  = GetCardWidth();
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

            var rt = go.GetComponent<RectTransform>();
            LayoutGridCard(rt, index, cardWidth, cardHeight);
            index++;
        }

        SetGridContentHeight(availableContent, index, cardHeight);
    }

    // =========================================================
    //  CHOSEN LIST (RIGHT) – VERTICAL LIST
    // =========================================================

    private void RefreshChosenList()
    {
        if (chosenContent == null || cardItemPrefab == null)
        {
            Debug.LogWarning("CardLibraryUI: ChosenContent or CardItemPrefab is not assigned.");
            return;
        }

        for (int i = chosenContent.childCount - 1; i >= 0; i--)
            Destroy(chosenContent.GetChild(i).gameObject);

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
            LayoutListCard(rt, index, cardHeight);
            index++;
        }

        SetListContentHeight(chosenContent, index, cardHeight);
        UpdateCounters();
    }

    // =========================================================
    //  LAYOUT HELPERS (NO LAYOUT GROUPS)
    // =========================================================

    private float GetCardHeight()
    {
        var rt = cardItemPrefab.GetComponent<RectTransform>();
        return rt != null ? rt.rect.height : 150f;
    }

    private float GetCardWidth()
    {
        var rt = cardItemPrefab.GetComponent<RectTransform>();
        return rt != null ? rt.rect.width : 100f;
    }

    // LEFT GRID
    private void LayoutGridCard(RectTransform cardRT, int index, float cardWidth, float cardHeight)
    {
        int col = index % Mathf.Max(1, gridColumns);
        int row = index / Mathf.Max(1, gridColumns);

        cardRT.anchorMin = new Vector2(0f, 1f);
        cardRT.anchorMax = new Vector2(0f, 1f);
        cardRT.pivot     = new Vector2(0f, 1f);

        cardRT.sizeDelta = new Vector2(cardWidth, cardHeight);

        float x = gridLeftPadding + col * (cardWidth + gridHorizontalSpacing);
        float y = -(gridTopPadding + row * (cardHeight + gridVerticalSpacing));
        cardRT.anchoredPosition = new Vector2(x, y);
    }

    private void SetGridContentHeight(RectTransform content, int count, float cardHeight)
    {
        int cols = Mathf.Max(1, gridColumns);
        int rows = Mathf.CeilToInt(count / (float)cols);

        float height = gridTopPadding + rows * (cardHeight + gridVerticalSpacing);
        if (height < 0) height = 0;

        Vector2 size = content.sizeDelta;
        size.y = height;
        content.sizeDelta = size;
    }

    // RIGHT LIST
    private void LayoutListCard(RectTransform cardRT, int index, float cardHeight)
    {
        cardRT.anchorMin = new Vector2(0f, 1f);
        cardRT.anchorMax = new Vector2(1f, 1f);
        cardRT.pivot     = new Vector2(0.5f, 1f);

        float leftPadding  = 10f;
        float rightPadding = 10f;

        cardRT.offsetMin = new Vector2(leftPadding, 0f);
        cardRT.offsetMax = new Vector2(-rightPadding, 0f);
        cardRT.sizeDelta = new Vector2(cardRT.sizeDelta.x, cardHeight);

        float y = -(listTopPadding + index * (cardHeight + listVerticalSpacing));
        cardRT.anchoredPosition = new Vector2(0f, y);
    }

    private void SetListContentHeight(RectTransform content, int count, float cardHeight)
    {
        float height = listTopPadding + count * (cardHeight + listVerticalSpacing);
        if (height < 0) height = 0;

        Vector2 size = content.sizeDelta;
        size.y = height;
        content.sizeDelta = size;
    }

    // =========================================================
    //  CLICK HANDLERS
    // =========================================================

    // Left → Right
    public void OnCardClickedFromAvailable(CardData data)
    {
        if (data == null) return;

        if (chosenIds.Add(data.id))
        {
            chosenCards.Add(data);
            RefreshChosenList();
            RefreshAvailableList();   // remove from left
        }
    }

    // Right → Left
    public void OnCardClickedFromChosen(CardData data)
    {
        if (data == null) return;

        if (chosenIds.Remove(data.id))
        {
            chosenCards.RemoveAll(c => c.id == data.id);
            RefreshChosenList();      // remove from right
            RefreshAvailableList();   // show back on left
        }
    }

    // =========================================================
    //  COUNTERS
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
    //  FILTER BUTTONS (TABS)
    // =========================================================

    public void ShowAll()
    {
        currentFilter = FilterMode.All;
        RefreshAvailableList();
    }

    public void ShowPolicies()
    {
        currentFilter = FilterMode.Policy;
        RefreshAvailableList();
    }

    public void ShowProjects()
    {
        currentFilter = FilterMode.Project;
        RefreshAvailableList();
    }

    public void ShowShortTerm()
    {
        currentFilter = FilterMode.ShortTerm;
        RefreshAvailableList();
    }

    // Optional: override dummy data externally
    public void SetCards(List<CardData> cards)
    {
        allCards = cards ?? new List<CardData>();
        chosenCards.Clear();
        chosenIds.Clear();
        RefreshChosenList();
        RefreshAvailableList();
    }
}
