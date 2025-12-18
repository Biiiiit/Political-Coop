using UnityEngine;

public class CardCreationController : MonoBehaviour
{
    public Assignedrisks previewCardRuntime;
    public ProjectCardUI previewCardUI;
    public RiskSelectionPanel riskSelectionPanel;

    void Start()
    {
        riskSelectionPanel.Close();
    }

    public void OnAddRiskButtonPressed()
    {
        riskSelectionPanel.Open(this);
    }

    public void AddRiskToPreview(Risk risk)
    {
        if (!previewCardRuntime.assignedRisks.Contains(risk))
        {
            previewCardRuntime.assignedRisks.Add(risk);

            previewCardUI.DisplayRiskIcons();
        }

        riskSelectionPanel.Close();
    }
}
