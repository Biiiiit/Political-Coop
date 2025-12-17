using UnityEngine;

public class ProjectCardUI : MonoBehaviour
{
    [Header("References")]
    public Transform negativePanel;
    public GameObject riskIconPrefab;

    public ProjectCardPreviewData PreviewData { get; private set; }

    void Awake()
    {
        PreviewData = new ProjectCardPreviewData();
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in negativePanel)
            Destroy(child.gameObject);

        foreach (Risk risk in PreviewData.addedRisks)
        {
            GameObject iconGO = Instantiate(riskIconPrefab, negativePanel);
            iconGO.GetComponent<RiskIconUI>().SetRisk(risk);
        }
    }

    public void ResetPreview()
    {
        PreviewData = new ProjectCardPreviewData();
        Refresh();
    }
}
