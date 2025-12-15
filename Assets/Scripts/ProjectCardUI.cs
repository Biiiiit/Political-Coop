using UnityEngine;

public class ProjectCardUI : MonoBehaviour
{
    [Header("References")]
    public Transform negativePanel;
    public GameObject riskIconPrefab;

    private ProjectCard cardData;

    public void SetCard(ProjectCard card)
    {
        cardData = card;
        PopulateRisks();
    }

    private void PopulateRisks()
    {
        // Clear existing icons
        foreach (Transform child in negativePanel)
            Destroy(child.gameObject);

        // Spawn new ones
        foreach (Risk risk in cardData.addedRisks)
        {
            GameObject iconGO = Instantiate(riskIconPrefab, negativePanel);
            iconGO.GetComponent<RiskIconUI>().SetRisk(risk);
        }
    }
}
