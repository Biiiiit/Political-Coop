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
        this.risk = risk;
        this.panel = panel;

        iconImage.sprite = risk.icon;
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        panel.SelectRisk(risk);
    }
}
