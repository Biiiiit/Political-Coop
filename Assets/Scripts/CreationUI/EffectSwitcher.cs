using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerIconUpdater : MonoBehaviour
{
    [Header("Player dropdown to listen to")]
    public TMP_Dropdown playerDropdown;

    [Header("UI Image to update")]
    public Image targetImage;

    [Header("Icons for each option")]
    public Sprite continuousIcon;
    public Sprite limitedIcon;

    private void Start()
    {
        // Initialize icon at start
        UpdateIcon(playerDropdown.value);

        // Add listener for runtime changes
        playerDropdown.onValueChanged.AddListener(UpdateIcon);
    }

    private void UpdateIcon(int index)
    {
        if (playerDropdown.options[index].text == "Continuous Effect")
        {
            targetImage.sprite = continuousIcon;
        }
        else if (playerDropdown.options[index].text == "Limited Effect")
        {
            targetImage.sprite = limitedIcon;
        }
        else
        {
            targetImage.sprite = null; // optional: hide if other values
        }
    }
}
