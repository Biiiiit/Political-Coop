using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RiskTileAssigner : MonoBehaviour
{
    public List<RiskTile> boardTiles;

    public bool AssignRiskToPlayer(string playerId, Risk risk)
    {
        var playerTiles = boardTiles.Where(t => t.ownerPlayerId == playerId).ToList();
        var emptyTiles = playerTiles.Where(t => t.IsEmpty).ToList();

        if (emptyTiles.Count == 0)
            return false;

        RiskTile selected = emptyTiles[Random.Range(0, emptyTiles.Count)];
        selected.currentRisk = risk;
        return true;
    }
}
