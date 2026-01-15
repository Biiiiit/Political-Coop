using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RiskCreationManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject createRiskPanel;
    public GameObject backgroundOverlay;

    [Header("Basic Risk Inputs")]
    public TMP_InputField nameInput;
    public TMP_InputField descriptionInput;
    public TMP_InputField disasterTagsInput; // Comma-separated tags

    [Header("Player Assignment")]
    public Toggle player1Toggle;
    public Toggle player2Toggle;
    public Toggle player3Toggle;
    public Toggle player4Toggle;

    [Header("Risk List")]
    public Transform risksListParent;
    public GameObject riskItemPrefab;

    private GameObject currentRisk;
    private bool justOpenedPanel = false;

    void Update()
    {
        // Monitor panel state after opening
        if (justOpenedPanel)
        {
            if (!createRiskPanel.activeSelf)
            {
                Debug.LogWarning("[RiskCreationManager] Panel was deactivated after OpenRiskPanel() was called!");
                Debug.LogWarning(System.Environment.StackTrace);
                justOpenedPanel = false;
            }
        }
    }

    void Start()
    {
        // Add listeners to enforce max 2 player selections
        if (player1Toggle != null) player1Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
        if (player2Toggle != null) player2Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
        if (player3Toggle != null) player3Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
        if (player4Toggle != null) player4Toggle.onValueChanged.AddListener(delegate { EnforceMaxSelection(); });
    }

    void EnforceMaxSelection()
    {
        Toggle[] toggles = new Toggle[] { player1Toggle, player2Toggle, player3Toggle, player4Toggle };

        int selectedCount = 0;
        foreach (var t in toggles)
            if (t.isOn) selectedCount++;

        if (selectedCount >= 2)
        {
            foreach (var t in toggles)
                if (!t.isOn)
                    t.interactable = false;
        }
        else
        {
            foreach (var t in toggles)
                t.interactable = true;
        }
    }

    public void OpenRiskPanel()
    {
        Debug.Log("[RiskCreationManager] OpenRiskPanel called");
        
        // Deselect the button that was just clicked to avoid EventSystem holding onto it
        EventSystem.current.SetSelectedGameObject(null);
        
        // Bring panel to front of rendering order
        createRiskPanel.transform.SetAsLastSibling();
        
        createRiskPanel.SetActive(true);
        Debug.Log($"[RiskCreationManager] Panel activated - activeSelf: {createRiskPanel.activeSelf}");
        justOpenedPanel = true;
        
        if (backgroundOverlay != null)
            backgroundOverlay.SetActive(true);

        // Reset all fields
        ResetInputs();
    }

    void ResetInputs()
    {
        if (nameInput != null) nameInput.text = "";
        if (descriptionInput != null) descriptionInput.text = "";
        if (disasterTagsInput != null) disasterTagsInput.text = "";
        
        if (player1Toggle != null) player1Toggle.isOn = false;
        if (player2Toggle != null) player2Toggle.isOn = false;
        if (player3Toggle != null) player3Toggle.isOn = false;
        if (player4Toggle != null) player4Toggle.isOn = false;
    }

    public void CloseRiskPanel()
    {
        // Deselect any button that might be selected
        EventSystem.current.SetSelectedGameObject(null);
        
        createRiskPanel.SetActive(false);
        if (backgroundOverlay != null)
            backgroundOverlay.SetActive(false);
    }

    public void SaveRisk()
    {
        // Validate inputs
        if (string.IsNullOrEmpty(nameInput.text))
        {
            Debug.LogWarning("[RiskCreationManager] Risk name cannot be empty!");
            return;
        }

        // Collect selected players
        List<int> selectedPlayers = new List<int>();
        if (player1Toggle.isOn) selectedPlayers.Add(1);
        if (player2Toggle.isOn) selectedPlayers.Add(2);
        if (player3Toggle.isOn) selectedPlayers.Add(3);
        if (player4Toggle.isOn) selectedPlayers.Add(4);

        if (selectedPlayers.Count == 0)
        {
            Debug.LogWarning("[RiskCreationManager] Select at least 1 player for this risk!");
            return;
        }

        // Create a new Risk ScriptableObject
        Risk newRisk = ScriptableObject.CreateInstance<Risk>();
        newRisk.name = nameInput.text; // Set the object's internal name
        newRisk.riskName = nameInput.text; // Set the display name field
        newRisk.description = descriptionInput.text;
        newRisk.category = string.Join(", ", selectedPlayers); // Store player assignments
        
        // Load default icon from Resources
        newRisk.icon = Resources.Load<Sprite>("RiskIcons/DefaultRiskIcon");
        if (newRisk.icon == null)
        {
            Debug.LogWarning("[RiskCreationManager] DefaultRiskIcon not found in Resources/RiskIcons/. Make sure the sprite is placed there.");
        }
        
        // Parse disaster tags (comma-separated)
        if (!string.IsNullOrEmpty(disasterTagsInput.text))
        {
            newRisk.disasterTags = disasterTagsInput.text.Split(',');
            // Trim whitespace from each tag
            for (int i = 0; i < newRisk.disasterTags.Length; i++)
                newRisk.disasterTags[i] = newRisk.disasterTags[i].Trim();
        }

        // Set default values for behavior (can be customized later)
        newRisk.severity = 1;
        newRisk.baseProbability = 0.02f;
        newRisk.escalationRate = 0.01f;
        newRisk.color = Color.white;

        // Register with RiskLibrary so it's available for card creation
        if (RiskLibrary.Instance != null)
        {
            RiskLibrary.Instance.RegisterRisk(newRisk);
            Debug.Log($"[RiskCreationManager] Risk '{newRisk.name}' registered to RiskLibrary for players: {string.Join(", ", selectedPlayers)}");
        }
        else
        {
            Debug.LogError("[RiskCreationManager] RiskLibrary.Instance not found!");
            return;
        }

        // Create visual representation in list
        if (riskItemPrefab == null)
        {
            Debug.LogError("[RiskCreationManager] riskItemPrefab not assigned!");
            return;
        }

        if (currentRisk != null)
            Destroy(currentRisk);

        currentRisk = Instantiate(riskItemPrefab, risksListParent);

        // Set risk name
        TMP_Text[] fields = currentRisk.GetComponentsInChildren<TMP_Text>();
        if (fields.Length >= 1)
        {
            fields[0].text = nameInput.text;
        }

        Debug.Log($"[RiskCreationManager] Risk '{nameInput.text}' created for players: {string.Join(", ", selectedPlayers)}");
        CloseRiskPanel();
    }
}
