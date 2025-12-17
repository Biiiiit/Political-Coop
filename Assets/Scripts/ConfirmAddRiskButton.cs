using UnityEngine;
using UnityEngine.UI;

public class ConfirmAddRiskButton : MonoBehaviour
{
    public RiskSelectionPanel riskPanel; // assign in inspector
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
            Debug.LogError("No Button component found on ConfirmAddRiskButton!");

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        if (riskPanel != null)
        {
            Debug.Log("Confirm Add Risk clicked");
            riskPanel.ConfirmSelection();
        }
        else
        {
            Debug.LogError("RiskSelectionPanel reference not set in ConfirmAddRiskButton!");
        }
    }
}
