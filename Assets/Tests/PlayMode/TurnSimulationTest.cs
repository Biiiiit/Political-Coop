using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Collections;

public class TurnSimulationTests
{
    [UnityTest]
    public IEnumerator FullTurn_CalamityFlow_Works()
    {
        // Managers
        var riskManagerObj = new GameObject();
        var riskManager = riskManagerObj.AddComponent<RiskManager>();

        var disasterManagerObj = new GameObject();
        var disasterManager = disasterManagerObj.AddComponent<DisasterManager>();

        // Risk with high trigger chance
        var risk = ScriptableObject.CreateInstance<Risk>();
        risk.baseProbability = 0.9f;
        risk.disasterTags = new[] { "flood" };
        riskManager.activeRisks.Add(risk);

        // Disaster connected to risk
        var disaster = ScriptableObject.CreateInstance<Disaster>();
        disaster.requiredRiskTags = new[] { "flood" };
        disaster.baseChance = 1f; // guaranteed
        disasterManager.allDisasters = new List<Disaster>() { disaster };

        // Simulate turn
        var triggeredRisks = riskManager.GetTriggeredRisks();
        var triggeredDisaster = disasterManager.CalculateDisaster(triggeredRisks);

        Assert.IsTrue(triggeredRisks.Count > 0);
        Assert.IsNotNull(triggeredDisaster);

        yield return null;
    }
}
