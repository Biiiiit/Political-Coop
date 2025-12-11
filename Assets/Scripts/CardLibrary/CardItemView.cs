using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleAndTypeText;
    [SerializeField] private Button button;

    private CardData data;
    private CardLibraryUI manager;
    private bool isChosenList;

    public void Init(CardData data, CardLibraryUI manager, bool isChosenList)
    {
        this.data = data;
        this.manager = manager;
        this.isChosenList = isChosenList;

        titleAndTypeText.text = $"{data.title} | {data.GetTypeLabel()}";

        button.onClick.RemoveAllListeners();
        if (isChosenList)
            button.onClick.AddListener(RemoveFromChosen);
        else
            button.onClick.AddListener(AddToChosen);
    }

    private void AddToChosen()
    {
        manager.OnCardClickedFromAvailable(data);
    }

    private void RemoveFromChosen()
    {
        manager.OnCardClickedFromChosen(data);
    }
}
