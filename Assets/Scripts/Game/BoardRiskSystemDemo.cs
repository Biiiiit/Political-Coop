using UnityEngine;

/// <summary>
/// Demo/Test script showing how to use the risk system on the board
/// Attach this to any GameObject in the scene to test risk placement
/// </summary>
public class BoardRiskSystemDemo : MonoBehaviour
{
    void Update()
    {
        // Demo controls
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestAddRandomRisks();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TestClearAllRisks();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DisplayBoardRiskSummary();
        }
    }

    private void TestAddRandomRisks()
    {
        if (BoardRiskManager.Instance == null)
        {
            Debug.LogError("[BoardRiskSystemDemo] BoardRiskManager not initialized!");
            return;
        }

        if (RiskLibrary.Instance == null || RiskLibrary.Instance.allRisks.Count == 0)
        {
            Debug.LogError("[BoardRiskSystemDemo] No risks in RiskLibrary!");
            return;
        }

        // Add random risks to random cells
        int riskCount = Random.Range(1, 5);
        for (int i = 0; i < riskCount; i++)
        {
            Risk randomRisk = RiskLibrary.Instance.allRisks[Random.Range(0, RiskLibrary.Instance.allRisks.Count)];
            int randomCell = BoardRiskManager.Instance.GetRandomCellId();

            BoardRiskManager.Instance.AddRiskToCell(randomCell, randomRisk);
        }

        Debug.Log($"[BoardRiskSystemDemo] Added {riskCount} random risks to board");
    }

    private void TestClearAllRisks()
    {
        if (BoardRiskManager.Instance == null)
        {
            Debug.LogError("[BoardRiskSystemDemo] BoardRiskManager not initialized!");
            return;
        }

        var cellsWithRisks = BoardRiskManager.Instance.GetCellsWithAnyRisk();
        foreach (int cellId in cellsWithRisks)
        {
            BoardRiskManager.Instance.RemoveRiskFromCell(cellId);
        }
        Debug.Log("[BoardRiskSystemDemo] All risks cleared from board");
    }

    private void DisplayBoardRiskSummary()
    {
        if (BoardRiskManager.Instance == null)
        {
            Debug.LogError("[BoardRiskSystemDemo] BoardRiskManager not initialized!");
            return;
        }

        var cellsWithRisks = BoardRiskManager.Instance.GetCellsWithAnyRisk();

        Debug.Log($"[BoardRiskSystemDemo] ===== BOARD RISK SUMMARY =====");
        Debug.Log($"Cells with risks: {cellsWithRisks.Count}");

        foreach (int cellId in cellsWithRisks)
        {
            Risk risk = BoardRiskManager.Instance.GetRiskOnCell(cellId);
            string riskName = risk != null ? risk.riskName : "None";
            Debug.Log($"  Cell {cellId}: {riskName}");
        }

        Debug.Log("[BoardRiskSystemDemo] ==============================");
    }
}
