using UnityEngine;
using TMPro;

public class DynamicTextSync : MonoBehaviour
{
    [Header("What input field controls this text?")]
    public TMP_InputField inputField;

    [Header("This text updates automatically")]
    public TMP_Text targetText;

    private void Start()
    {
        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        inputField.onValueChanged.AddListener(UpdateText);

        UpdateText(inputField.text); // initialize
    }

    void UpdateText(string value)
    {
        targetText.text = value;
    }
}
