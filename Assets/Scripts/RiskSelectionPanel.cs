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
        if (selectedRisk == null)
            return;

        caller.AddRiskToPreview(selectedRisk);
        Close();
    }
}
