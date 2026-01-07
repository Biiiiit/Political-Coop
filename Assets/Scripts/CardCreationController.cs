using UnityEngine;

public class CardCreationController : MonoBehaviour
{
    public static CardCreationController Instance { get; private set; }

    // Dynamically assigned at runtime
    private Assignedrisks previewCardRuntime;
    private ProjectCardUI previewCardUI;
    public RiskSelectionPanel riskSelectionPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        riskSelectionPanel.Close();
    }

    /// <summary>
    /// Called by the card prefab when it's instantiated to register itself
    /// </summary>
    public void RegisterCard(ProjectCardUI cardUI, Assignedrisks cardRisks)
    {
        previewCardUI = cardUI;
        previewCardRuntime = cardRisks;

        Debug.Log("[CardCreation] Card registered");

        // Clear any design-time risks
        previewCardRuntime.assignedRisks.Clear();
        previewCardUI.DisplayRiskIcons();
    }

    public void OnAddRiskButtonPressed()
    {
        riskSelectionPanel.Open(this);
    }

    public void AddRiskToPreview(Risk risk)
    {
        Debug.Log($"[CardCreation] AddRiskToPreview called with: {risk.name}");
        
        if (previewCardRuntime == null)
        {
            Debug.LogError("[CardCreation] previewCardRuntime is NULL!");
            return;
        }

        if (!previewCardRuntime.assignedRisks.Contains(risk))
        {
            Debug.Log($"[CardCreation] Adding {risk.name} to card");
            previewCardRuntime.assignedRisks.Add(risk);
            previewCardUI.DisplayRiskIcons();
            Debug.Log($"[CardCreation] Card now has {previewCardRuntime.assignedRisks.Count} risks");
        }
        else
        {
            Debug.Log($"[CardCreation] {risk.name} already on card, skipping");
        }

        riskSelectionPanel.Close();
    }
}
