using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class RiskLogicTests
{
    private Risk CreateRisk(float baseProb, float escalation, string[] tags)
    {
        var risk = ScriptableObject.CreateInstance<Risk>();
        risk.baseProbability = baseProb;
        risk.escalationRate = escalation;
        risk.disasterTags = tags;
        return risk;
    }

    private Disaster CreateDisaster(string[] tags, float chance)
    {
        var disaster = ScriptableObject.CreateInstance<Disaster>();
        disaster.requiredRiskTags = tags;
        disaster.baseChance = chance;
        return disaster;
    }

    [Test]
    public void Risk_ProbabilityEscalatesCorrectly()
    {
        var risk = CreateRisk(0.1f, 0.05f, new string[]{});
        var manager = new GameObject().AddComponent<RiskManager>();
        manager.activeRisks.Add(risk);

        manager.AdvanceTurn(); // Turn 2
        float expected = 0.1f + (0.05f * 2);

        Assert.AreEqual(expected, risk.baseProbability + risk.escalationRate * manager.currentTurn);
    }

    [Test]
    public void Disaster_TriggersWhenTagsMatch()
    {
        var risk = CreateRisk(0.1f, 0f, new[] { "fire" });
        var disaster = CreateDisaster(new[] { "fire" }, 1f); // guaranteed

        var disasterManager = new GameObject().AddComponent<DisasterManager>();
        disasterManager.allDisasters = new List<Disaster>() { disaster };

        var triggered = disasterManager.CalculateDisaster(new List<Risk>() { risk });

        Assert.IsNotNull(triggered);
        Assert.AreEqual(disaster, triggered);
    }

    [Test]
    public void Tile_AssignsRiskToCorrectPlayer()
    {
        var tile1 = new GameObject().AddComponent<RiskTile>();
        tile1.ownerPlayerId = "P1";

        var tile2 = new GameObject().AddComponent<RiskTile>();
        tile2.ownerPlayerId = "P1";

        var tile3 = new GameObject().AddComponent<RiskTile>();
        tile3.ownerPlayerId = "P2";

        var assigner = new GameObject().AddComponent<RiskTileAssigner>();
        assigner.boardTiles = new List<RiskTile>() { tile1, tile2, tile3 };

        var risk = CreateRisk(0.1f, 0f, new string[]{});

        bool result = assigner.AssignRiskToPlayer("P1", risk);

        Assert.IsTrue(result);
        Assert.IsTrue(tile1.currentRisk == risk || tile2.currentRisk == risk);
        Assert.IsNull(tile3.currentRisk);
    }
}
