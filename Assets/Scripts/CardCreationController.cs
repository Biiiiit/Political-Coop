using UnityEngine;


public class CardCreationController : MonoBehaviour
{
    public ProjectCard currentCard;
    public ProjectCardUI previewCardUI;
    public RiskSelectionPanel riskSelectionPanel;

    public void OnAddRiskButtonPressed()
    {
        riskSelectionPanel.Open(this);
    }

    public void AddRiskToCard(Risk risk)
    {
        if (!currentCard.addedRisks.Contains(risk))
        {
            currentCard.addedRisks.Add(risk);
            previewCardUI.SetCard(currentCard);
        }
    }
}

