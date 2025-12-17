using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RiskSelectionButton : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI label;
    public Button button;

    private Risk risk;
    private RiskSelectionPanel panel;

    public void Initialize(Risk risk, RiskSelectionPanel panel)
    {
        this.risk = risk;
        this.panel = panel;

        icon.sprite = risk.icon;
        label.text = risk.name;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        panel.SelectRisk(risk);
    }
}
