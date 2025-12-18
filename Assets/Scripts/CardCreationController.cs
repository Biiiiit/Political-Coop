using UnityEngine;

public class CardCreationController : MonoBehaviour
{
    public ProjectCardUI previewCardUI;
    public RiskSelectionPanel riskSelectionPanel;

    private ProjectCardPreviewData Preview => previewCardUI.PreviewData;

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
        if (!Preview.addedRisks.Contains(risk))
        {
            Preview.addedRisks.Add(risk);
            previewCardUI.DisplayRiskIcons();
            riskSelectionPanel.Close();
        }
    }
}
