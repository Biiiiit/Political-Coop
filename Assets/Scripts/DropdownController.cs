using UnityEngine;
using TMPro;

public class TMPDropdownController : MonoBehaviour
{
    public TMP_Dropdown mainDropdown;
    public TMP_Dropdown conditionalDropdown;

    // Optional: index that triggers the conditional dropdown
    public int triggerIndex = 0; // e.g., second option in MainDropdown

    void Start()
    {
        // Subscribe to main dropdown changes
        mainDropdown.onValueChanged.AddListener(OnMainDropdownChanged);
    }

    void OnMainDropdownChanged(int index)
    {
        // Show conditional dropdown only if the selected index matches
        conditionalDropdown.gameObject.SetActive(index == triggerIndex);

        // Optional: reset selection when hiding
        if (index != triggerIndex)
            conditionalDropdown.value = 0;
    }
}
