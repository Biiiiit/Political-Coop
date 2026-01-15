using UnityEngine;
using System.Collections.Generic;

public class RiskLibrary : MonoBehaviour
{
    public static RiskLibrary Instance { get; private set; }

    [Header("All Risk Assets")]
    public List<Risk> allRisks = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (allRisks.Count == 0)
        {
            Risk[] risks = Resources.LoadAll<Risk>("Risks");
            allRisks.AddRange(risks);
        }
    }

    public void RegisterRisk(Risk risk)
    {
        if (!allRisks.Contains(risk))
            allRisks.Add(risk);
    }
}
