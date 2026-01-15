using UnityEngine;

public class ProjectCardUI : MonoBehaviour
{
    public Transform negativePanel;
    public GameObject riskIconPrefab;

    private Assignedrisks assignedrisks;

    void Awake()
    {
        Debug.Log($"[ProjectCardUI] Awake called on {gameObject.name}");
        
        // Get all components on this GameObject to debug
        Component[] allComponents = GetComponents<Component>();
        Debug.Log($"[ProjectCardUI] Components on {gameObject.name}: {string.Join(", ", System.Array.ConvertAll(allComponents, c => c.GetType().Name))}");
        
        assignedrisks = GetComponent<Assignedrisks>();
        
        if (assignedrisks == null)
        {
            Debug.LogError($"[ProjectCardUI] Assignedrisks NOT found on {gameObject.name}! Check Inspector to ensure component is attached.");
            return;
        }
        
        Debug.Log($"[ProjectCardUI] Assignedrisks found on {gameObject.name}");
        
        if (negativePanel == null)
        {
            Debug.LogError("[ProjectCardUI] negativePanel reference not set in Inspector!");
            return;
        }
        
        DisplayRiskIcons();
    }

    public void DisplayRiskIcons()
    {
        if (assignedrisks == null)
        {
            Debug.LogError("[ProjectCardUI] assignedrisks is NULL!");
            return;
        }

        if (negativePanel == null)
        {
            Debug.LogError("[ProjectCardUI] negativePanel is NULL!");
            return;
        }

        if (riskIconPrefab == null)
        {
            Debug.LogError("[ProjectCardUI] riskIconPrefab is not assigned in Inspector!");
            return;
        }

        // Clear existing icons
        foreach (Transform child in negativePanel)
            DestroyImmediate(child.gameObject, true);

        // Add new icons for each risk
        foreach (Risk risk in assignedrisks.GetRisks())
        {
            GameObject icon = Instantiate(riskIconPrefab, negativePanel);
            RiskIconUI riskUI = icon.GetComponent<RiskIconUI>();
            if (riskUI != null)
                riskUI.SetRisk(risk);
            else
                Debug.LogError("[ProjectCardUI] riskIconPrefab missing RiskIconUI component!");
        }
    }
}
