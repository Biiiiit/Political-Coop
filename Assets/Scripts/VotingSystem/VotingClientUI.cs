using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VotingClientUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Transform cardAnchor;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private string currentCardId;

    void Start()
    {
        gameObject.SetActive(false);

        headerText.text = "Voting Time!";

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    public void ShowVoting(GameObject cardPrefab, string cardId)
    {
        gameObject.SetActive(true);
        currentCardId = cardId;

        ClearChildren(cardAnchor);

        if (cardPrefab != null)
        {
            Instantiate(cardPrefab, cardAnchor);
        }
    }

    private void OnYesClicked()
    {
        Debug.Log("VOTED YES on card " + currentCardId);
        DisableButtons();
    }

    private void OnNoClicked()
    {
        Debug.Log("VOTED NO on card " + currentCardId);
        DisableButtons();
    }

    private void DisableButtons()
    {
        yesButton.interactable = false;
        noButton.interactable = false;
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
