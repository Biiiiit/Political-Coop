using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class SubPanel
{
    public GameObject panelObject;

    [Header("Special Cases")]
    public Toggle[] inputToggles;          // Toggles controlling conditional inputs
    public TMP_InputField[] conditionalInputs;

    [HideInInspector] public TMP_InputField[] inputFields;
    [HideInInspector] public TMP_Dropdown[] dropdowns;
    [HideInInspector] public Toggle[] toggles;

    public void Init()
    {
        if (panelObject == null) return;
        inputFields = panelObject.GetComponentsInChildren<TMP_InputField>();
        dropdowns = panelObject.GetComponentsInChildren<TMP_Dropdown>();
        toggles = panelObject.GetComponentsInChildren<Toggle>();
    }

    public List<string> GetValues()
    {
        List<string> values = new List<string>();

        // Save dropdown TEXT (not index)
        foreach (var d in dropdowns)
        {
            values.Add(d.options[d.value].text);
        }

        // Handle toggle + number logic explicitly
        if (inputToggles != null && conditionalInputs != null)
        {
            for (int i = 0; i < inputToggles.Length && i < conditionalInputs.Length; i++)
            {
                if (!inputToggles[i].isOn) continue;

                string number = conditionalInputs[i].text;

                if (string.IsNullOrWhiteSpace(number)) continue;

                if (i == 0)
                    values.Add($"Total number of times {number}");
                else if (i == 1)
                    values.Add($"Every {number} turns");
            }
        }

        // Save remaining input fields that are NOT conditional
        foreach (var input in inputFields)
        {
            if (conditionalInputs != null && System.Array.Exists(conditionalInputs, c => c == input))
                continue;

            if (!string.IsNullOrWhiteSpace(input.text))
                values.Add(input.text);
        }

        return values;
    }

    public void Reset()
    {
        foreach (var d in dropdowns) d.value = 0;
        foreach (var t in toggles) t.isOn = false;
        foreach (var i in inputFields) i.text = "";
    }
}

[System.Serializable]
public class MainPanel
{
    public GameObject panelObject;
    public SubPanel[] subPanels;

    public void Init()
    {
        if (subPanels == null) return;
        foreach (var sub in subPanels)
            sub.Init();
    }

    public void ShowSubPanel(int index)
    {
        for (int i = 0; i < subPanels.Length; i++)
        {
            if (subPanels[i].panelObject != null)
                subPanels[i].panelObject.SetActive(i == index);
        }
    }
}

[System.Serializable]
public class EffectData
{
    public string effectName;
    public string effectDescription;
    public int mainDropdownValue;
    public int subDropdownValue;
    public List<string> panelInputs;
}

public class EffectManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject createEffectPanel;
    public GameObject backgroundOverlay;

    [Header("Common Inputs")]
    public TMP_InputField nameInput;
    public TMP_InputField descriptionInput;

    [Header("Dropdowns")]
    public TMP_Dropdown mainDropdown;
    public TMP_Dropdown subDropdown;

    [Header("Main Panels")]
    public MainPanel[] mainPanels;

    private Dictionary<int, Dictionary<int, EffectData>> savedEffects = new();

    void Awake()
    {
        foreach (var main in mainPanels)
            main.Init();

        mainDropdown.onValueChanged.AddListener(OnMainDropdownChanged);
        subDropdown.onValueChanged.AddListener(OnSubDropdownChanged);

        OpenEffectPanel();
    }

    public void OpenEffectPanel()
    {
        createEffectPanel.SetActive(true);
        if (backgroundOverlay != null) backgroundOverlay.SetActive(true);
        ResetAllInputs();
        UpdatePanels(mainDropdown.value);
        UpdateSubPanels(subDropdown.value);
    }

    public void CloseEffectPanel()
    {
        createEffectPanel.SetActive(false);
        if (backgroundOverlay != null) backgroundOverlay.SetActive(false);
    }

    void ResetAllInputs()
    {
        nameInput.text = "";
        descriptionInput.text = "";
        mainDropdown.value = 0;
        subDropdown.value = 0;

        foreach (var main in mainPanels)
        {
            foreach (var sub in main.subPanels)
                sub.Reset();
        }
    }

    void OnMainDropdownChanged(int value)
    {
        UpdatePanels(value);
        subDropdown.value = 0;
        subDropdown.RefreshShownValue();
        UpdateSubPanels(subDropdown.value);
    }

    void OnSubDropdownChanged(int value)
    {
        UpdateSubPanels(value);
    }

    void UpdatePanels(int mainIndex)
    {
        for (int i = 0; i < mainPanels.Length; i++)
        {
            if (mainPanels[i].panelObject != null)
                mainPanels[i].panelObject.SetActive(i == mainIndex);
        }
    }

    void UpdateSubPanels(int subIndex)
    {
        if (mainDropdown.value < mainPanels.Length)
            mainPanels[mainDropdown.value].ShowSubPanel(subIndex);
    }

    public void SaveEffect()
    {
        int mainIndex = mainDropdown.value;
        int subIndex = subDropdown.value;

        if (!savedEffects.ContainsKey(mainIndex))
            savedEffects[mainIndex] = new Dictionary<int, EffectData>();

        var data = new EffectData
        {
            effectName = nameInput.text,
            effectDescription = descriptionInput.text,
            mainDropdownValue = mainIndex,
            subDropdownValue = subIndex,
            panelInputs = mainPanels[mainIndex].subPanels[subIndex].GetValues()
        };

        savedEffects[mainIndex][subIndex] = data;

        string subType = subIndex switch
        {
            0 => "Continuous",
            1 => "Limited",
            2 => "SingleUse",
            _ => "Unknown"
        };

        // Filter out empty strings and raw toggles
        var filteredInputs = new List<string>();
        foreach (var input in data.panelInputs)
        {
            if (!string.IsNullOrWhiteSpace(input) && input != "True" && input != "False")
                filteredInputs.Add(input);
        }

        Debug.Log($"Saved Effect: {subType}, {data.effectName}, {data.effectDescription}, Inputs: {string.Join(", ", filteredInputs)}");
        CloseEffectPanel();
    }
}
