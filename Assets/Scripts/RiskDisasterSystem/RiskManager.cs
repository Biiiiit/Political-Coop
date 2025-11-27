using System.Collections.Generic;
using UnityEngine;

public class RiskManager : MonoBehaviour
{
    public List<Risk> activeRisks = new List<Risk>();
    public int currentTurn = 1;

    public void AdvanceTurn()
    {
        currentTurn++;
    }

    // Determine which risks trigger damage during a calamity
    public List<Risk> GetTriggeredRisks()
    {
        List<Risk> triggered = new List<Risk>();

        foreach (Risk risk in activeRisks)
        {
            float probability = risk.baseProbability + (risk.escalationRate * currentTurn);

            if (Random.value < probability)
                triggered.Add(risk);
        }

        return triggered;
    }

    public void AddRisk(Risk risk)
    {
        activeRisks.Add(risk);
    }
}
