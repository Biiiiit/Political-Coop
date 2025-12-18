using UnityEngine;
using UnityEngine.UI;

public class CardCreationAddRiskButton : MonoBehaviour
{
    public Image iconImage;
    public Button button;

    private Risk risk;
    private RiskSelectionPanel panel;

    public void Setup(Risk risk, RiskSelectionPanel panel)
    {
        if(button == null)
        {
            Debug.Log("no button assigned");
        }
        this.risk = risk;
        this.panel = panel;

        iconImage.sprite = risk.icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        Debug.Log("risk clicked");
        panel.SelectRisk(risk);
    }
}
