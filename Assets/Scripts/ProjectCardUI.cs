using UnityEngine;

public class ProjectCardUI : MonoBehaviour
{
    [Header("References")]
    public Transform negativePanel;           
    public GameObject riskIconPrefabForCard;

    public ProjectCardPreviewData PreviewData { get; private set; }

    void Awake()
    {
        PreviewData = new ProjectCardPreviewData();
        DisplayRiskIcons();
    }

    public void DisplayRiskIcons()
    {
        foreach (Transform child in negativePanel)
            Destroy(child.gameObject);

        foreach (Risk risk in PreviewData.addedRisks)
        {
            GameObject iconGO = Instantiate(riskIconPrefabForCard, negativePanel);

            RectTransform rt = iconGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;

            RiskIconUI iconUI = iconGO.GetComponent<RiskIconUI>();
            if (iconUI != null)
                iconUI.SetRisk(risk);
        }
    }

    public void ResetPreview()
    {
        PreviewData = new ProjectCardPreviewData();
        DisplayRiskIcons();
    }
}
