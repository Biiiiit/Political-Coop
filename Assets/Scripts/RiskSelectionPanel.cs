using UnityEngine;
using System.Collections.Generic;

public class RiskSelectionPanel : MonoBehaviour
{
    public Transform contentParent;
    public CardCreationAddRiskButton riskButtonPrefab;

    private CardCreationController caller;

    public void Open(CardCreationController controller)
    {
        caller = controller;
        gameObject.SetActive(true);

        Populate(RiskLibrary.Instance.AllRisks);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void Populate(List<Risk> risks)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (Risk risk in risks)
        {
            CardCreationAddRiskButton button = Instantiate(riskButtonPrefab, contentParent);
            button.Setup(risk, this);
        }
    }

    public void SelectRisk(Risk risk)
    {
        caller.AddRiskToCard(risk);
        Close();
    }
}
