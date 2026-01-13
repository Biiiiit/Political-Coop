using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles logic for resolving cards to board cells
/// Determines which cells receive risks when a card is accepted
/// </summary>
public class CardResolutionResolver : MonoBehaviour
{
    public enum PlacementStrategy
    {
        Random,           // Place on random cells
        RoleSpecific,     // Place on cells related to player role
        PlayerChoice,     // Let player choose (not implemented here)
    }

    [Header("Placement Settings")]
    public PlacementStrategy strategy = PlacementStrategy.Random;
    [Range(1, 5)] public int risksPerCard = 1;

    /// <summary>
    /// Resolves a card's risks to board cells
    /// </summary>
    public void ResolveCardToBoard(Assignedrisks cardRisks, Role playerRole)
    {
        if (cardRisks == null)
        {
            Debug.LogWarning("[CardResolutionResolver] cardRisks is null!");
            return;
        }

        if (BoardRiskManager.Instance == null)
        {
            Debug.LogError("[CardResolutionResolver] BoardRiskManager.Instance not found!");
            return;
        }

        IReadOnlyList<Risk> risks = cardRisks.GetRisks();

        if (risks.Count == 0)
        {
            Debug.Log("[CardResolutionResolver] Card has no risks to resolve");
            return;
        }

        foreach (Risk risk in risks)
        {
            List<int> targetCells = DeterminePlacementCells(playerRole, risk);

            foreach (int cellId in targetCells)
            {
                BoardRiskManager.Instance.AddRiskToCell(cellId, risk);
            }

            Debug.Log($"[CardResolutionResolver] Risk '{risk.riskName}' resolved to {targetCells.Count} cell(s)");
        }
    }

    /// <summary>
    /// Determines which cells should receive a risk based on strategy
    /// </summary>
    private List<int> DeterminePlacementCells(Role playerRole, Risk risk)
    {
        List<int> targetCells = new List<int>();

        switch (strategy)
        {
            case PlacementStrategy.Random:
                targetCells = BoardRiskManager.Instance.GetRandomCells(risksPerCard);
                break;

            case PlacementStrategy.RoleSpecific:
                targetCells = GetRoleSpecificCells(playerRole, risksPerCard);
                break;

            default:
                targetCells = BoardRiskManager.Instance.GetRandomCells(risksPerCard);
                break;
        }

        return targetCells;
    }

    private List<int> GetRoleSpecificCells(Role role, int count)
    {
        // TODO: Map roles to specific board regions
        // For now, just return random cells

        Debug.Log($"[CardResolutionResolver] Using role-specific placement for {role}");
        return BoardRiskManager.Instance.GetRandomCells(count);
    }
}
