using UnityEngine;
using System.Collections.Generic;

public class RiskLibrary : MonoBehaviour
{
    public static RiskLibrary Instance { get; private set; }

    public List<Risk> AllRisks = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterRisk(Risk risk)
    {
        if (!AllRisks.Contains(risk))
            AllRisks.Add(risk);
    }
}
