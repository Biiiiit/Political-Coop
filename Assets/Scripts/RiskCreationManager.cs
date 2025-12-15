using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RiskCreationManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject createRiskPanel;
    public GameObject backgroundOverlay;

    [Header("Inputs")]
    public Toggle player1Toggle;
    public Toggle player2Toggle;
    public Toggle player3Toggle;
    public Toggle player4Toggle;
    public TMP_InputField nameInput;
    public TMP_InputField descriptionInput;

    [Header("Risk List")]
    public Transform risksListParent;
    public GameObject riskItemPrefab; // prefab with CircleContainer
    public GameObject circlePrefab;   // prefab for a single circle

    private GameObject currentRisk;

    void Start()
    {
        createRiskPanel.SetActive(false);
        if (backgroundOverlay != null)
            backgroundOverlay.GetComponent<Button>().onClick.AddListener(CloseRiskPanel);

        // Add listeners to enforce max 2 selections
        player1Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
        player2Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
        player3Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
        player4Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
    }

    void EnforceMaxSelection()
    {
        // Gather toggles
        Toggle[] toggles = new Toggle[] { player1Toggle, player2Toggle, player3Toggle, player4Toggle };

        // Count how many are currently on
        int selectedCount = 0;
        foreach (var t in toggles)
            if (t.isOn) selectedCount++;

        // If more than 2, disable unchecked toggles
        if (selectedCount >= 2)
        {
            foreach (var t in toggles)
                if (!t.isOn)
                    t.interactable = false;
        }
        else
        {
            // Re-enable all toggles if less than 2 selected
            foreach (var t in toggles)
                t.interactable = true;
        }
    }

    public void OpenRiskPanel()
    {
        createRiskPanel.SetActive(true);
        if (backgroundOverlay != null)
            backgroundOverlay.SetActive(true);

        // Reset all fields
        nameInput.text = "";
        descriptionInput.text = "";
        player1Toggle.isOn = false;
        player2Toggle.isOn = false;
        player3Toggle.isOn = false;
        player4Toggle.isOn = false;
    }

    public void CloseRiskPanel()
    {
        createRiskPanel.SetActive(false);
        if (backgroundOverlay != null)
            backgroundOverlay.SetActive(false);
    }

    public void SaveRisk()
    {
        // Collect selected players
        List<int> selectedPlayers = new List<int>();
        if (player1Toggle.isOn) selectedPlayers.Add(1);
        if (player2Toggle.isOn) selectedPlayers.Add(2);
        if (player3Toggle.isOn) selectedPlayers.Add(3);
        if (player4Toggle.isOn) selectedPlayers.Add(4);

        // Prevent creating a risk if none are selected
        if (selectedPlayers.Count == 0)
        {
            Debug.LogWarning("Select at least 1 player to create a risk!");
            return;
        }

        // Only 1 risk per card
        if (currentRisk != null)
            Destroy(currentRisk);

        // Instantiate risk prefab
        currentRisk = Instantiate(riskItemPrefab, risksListParent);

        // Set name and description
        TMP_Text[] fields = currentRisk.GetComponentsInChildren<TMP_Text>();
        if (fields.Length >= 3)
        {
            fields[1].text = nameInput.text;
            fields[2].text = descriptionInput.text;
        }

        // Circle container
        Transform circleContainer = currentRisk.transform.Find("CircleContainer");
        if (circleContainer == null)
        {
            Debug.LogError("Risk prefab must have a child named 'CircleContainer'");
            return;
        }

        // Spawn a circle for each selected player
        foreach (int player in selectedPlayers)
        {
            GameObject circle = Instantiate(circlePrefab, circleContainer);
            TMP_Text circleText = circle.GetComponentInChildren<TMP_Text>();
            if (circleText != null)
                circleText.text = player.ToString();
        }

        CloseRiskPanel();
    }
}
