using UnityEngine;
using TMPro;

public class DropdownTextSwitcher : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TMP_Text targetText;

    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
        OnDropdownChanged(dropdown.value); // initialize
    }

    void OnDropdownChanged(int index)
    {
        // Dropdown options 0–3 correspond to 1–4
        switch (index)
        {
            case 0:
                targetText.text = "1";
                break;

            case 1:
                targetText.text = "2";
                break;

            case 2:
                targetText.text = "3";
                break;

            case 3:
                targetText.text = "4";
                break;
        }
    }
}
