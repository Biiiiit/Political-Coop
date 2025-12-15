using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [Header("Main card type dropdown")]
    public TMP_Dropdown cardTypeDropdown;

    [Header("Card type panels")]
    public GameObject[] cardTypes; // assign panels in same order as dropdown

    [Header("Player dropdown")]
    public TMP_Dropdown playerDropdown;

    [Header("Default options for Type 1")]
    public List<string> type1Options = new List<string> { "Player 1", "Player 2", "Player 3", "Player 4" };

    private void Start()
    {
        UpdateUI(cardTypeDropdown.value);
        cardTypeDropdown.onValueChanged.AddListener(UpdateUI);
    }

    private void UpdateUI(int index)
    {
        // Show only the selected card type panel
        for (int i = 0; i < cardTypes.Length; i++)
            cardTypes[i].SetActive(i == index);

        // Update player dropdown based on selected card type
        switch (index)
        {
            case 0: // Type1
                playerDropdown.gameObject.SetActive(true);
                SetDropdownOptions(playerDropdown, type1Options);
                break;

            case 1: // Type2
                playerDropdown.gameObject.SetActive(true);
                SetDropdownOptions(playerDropdown, new List<string> { "Continuous Effect", "Limited Effect" });
                break;

            case 2: // Type3
                playerDropdown.gameObject.SetActive(false);
                break;
        }
    }

    private void SetDropdownOptions(TMP_Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = 0;
        dropdown.RefreshShownValue();

        // Force the OnValueChanged event to fire
        dropdown.onValueChanged.Invoke(dropdown.value);
    }
}
