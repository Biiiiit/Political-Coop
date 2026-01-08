using UnityEngine;
using System.Collections.Generic;

public class RiskSelectionPanel : MonoBehaviour
{
    public Transform contentParent;
    public CardCreationAddRiskButton riskButtonPrefab;

    private CardCreationController caller;
    private Risk selectedRisk;

    public void Open(CardCreationController controller)
    {
        caller = controller;
        selectedRisk = null;

        gameObject.SetActive(true);
        Populate(RiskLibrary.Instance.allRisks);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void Populate(List<Risk> risks)
    {
        Debug.Log("Risks count: " + risks.Count);
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (Risk risk in risks)
        {
            CardCreationAddRiskButton button =
                Instantiate(riskButtonPrefab, contentParent);

            button.Setup(risk, this);
        }
    }

    public void SelectRisk(Risk risk)
    {
        selectedRisk = risk;
        Debug.Log($"Selected risk: {risk.name}");
    }

    public void ConfirmSelection()
    {
        Debug.Log("[RiskSelectionPanel] ConfirmSelection called");
        
        if (selectedRisk == null)
        {
            Debug.LogWarning("[RiskSelectionPanel] selectedRisk is NULL!");
            return;
        }

        if (caller == null)
        {
            Debug.LogError("[RiskSelectionPanel] caller (CardCreationController) is NULL!");
            return;
        }

        Debug.Log($"[RiskSelectionPanel] Adding risk: {selectedRisk.name} to preview");
        caller.AddRiskToPreview(selectedRisk);
        Close();
    }
}
