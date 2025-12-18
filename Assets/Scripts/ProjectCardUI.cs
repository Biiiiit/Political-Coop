using UnityEngine;

public class ProjectCardUI : MonoBehaviour
{
    public Transform negativePanel;
    public GameObject riskIconPrefab;

    private Assignedrisks assignedrisks;

    void Awake()
    {
        assignedrisks = GetComponent<Assignedrisks>();
        DisplayRiskIcons();
    }

    public void DisplayRiskIcons()
    {
        foreach (Transform child in negativePanel)
            Destroy(child.gameObject);

        foreach (Risk risk in assignedrisks.GetRisks())
        {
            GameObject icon = Instantiate(riskIconPrefab, negativePanel);
            icon.GetComponent<RiskIconUI>().SetRisk(risk);
        }
    }
}
