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

    [Header("UI elements to toggle")]
    public GameObject panelToHide;      // The panel you want to hide
    public GameObject buttonToHide;     // The button you want to hide
    public GameObject button1ToShow;    // First new button
    public GameObject button2ToShow;    // Second new button

    private void Start()
    {
        // Initialize UI
        UpdateUI(cardTypeDropdown.value);

        // Listen to main dropdown changes
        cardTypeDropdown.onValueChanged.AddListener(UpdateUI);
    }

    private void UpdateUI(int index)
    {
        // Show only the selected card type panel
        for (int i = 0; i < cardTypes.Length; i++)
            cardTypes[i].SetActive(i == index);

        switch (index)
        {
            case 0: // Type1
                playerDropdown.gameObject.SetActive(true);
                SetDropdownOptions(playerDropdown, type1Options);
                ResetUIElements();
                break;

            case 1: // Type2 → hide panel and button, show new buttons
                playerDropdown.gameObject.SetActive(false);

                // Hide old elements
                panelToHide.SetActive(false);
                buttonToHide.SetActive(false);

                // Show new buttons
                button1ToShow.SetActive(true);
                button2ToShow.SetActive(true);
                break;

            case 2: // Type3
                playerDropdown.gameObject.SetActive(false);
                ResetUIElements();
                break;
        }
    }

    private void SetDropdownOptions(TMP_Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    private void ResetUIElements()
    {
        // Show old elements
        panelToHide.SetActive(true);
        buttonToHide.SetActive(true);

        // Hide new buttons
        button1ToShow.SetActive(false);
        button2ToShow.SetActive(false);
    }
}
