using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI titleAndTypeText;
    [SerializeField] private Button button;

    private CardData data;
    private CardLibraryUI manager;
    private bool isChosenList;

    private Canvas canvas;
    private RectTransform dragRect;
    private GameObject dragObject;

    public void Init(CardData data, CardLibraryUI manager, bool isChosenList)
    {
        this.data = data;
        this.manager = manager;
        this.isChosenList = isChosenList;

        if (titleAndTypeText != null)
            titleAndTypeText.text = $"{data.title} | {data.GetTypeLabel()}";

        // click behaviour (still works)
        button.onClick.RemoveAllListeners();
        if (isChosenList)
            button.onClick.AddListener(() => manager.OnCardClickedFromChosen(data));
        else
            button.onClick.AddListener(() => manager.OnCardClickedFromAvailable(data));

        canvas = manager.MainCanvas;   // we’ll expose this from CardLibraryUI
    }

    // --------- Drag & Drop ---------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // create visual clone that will follow the cursor
        dragObject = Instantiate(gameObject, canvas.transform);
        dragObject.name = "Drag_" + gameObject.name;

        // remove CardItemView from the clone so it won't receive events
        var cloneView = dragObject.GetComponent<CardItemView>();
        if (cloneView != null)
            Destroy(cloneView);

        // let raycasts go through the ghost to the drop areas
        var cg = dragObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        dragRect = dragObject.GetComponent<RectTransform>();
        dragRect.sizeDelta = GetComponent<RectTransform>().sizeDelta;

        UpdateDragPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragRect == null) return;
        UpdateDragPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragObject != null)
            Destroy(dragObject);

        if (manager != null)
            manager.HandleCardDrop(data, isChosenList, eventData.position);
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, eventData.position, canvas.worldCamera, out var localPoint))
        {
            dragRect.localPosition = localPoint;
        }
    }
}
