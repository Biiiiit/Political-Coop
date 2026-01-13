using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Centralized manager for risks on board cells
/// Syncs risk placement across multiplayer clients
/// </summary>
public class BoardRiskManager : MonoBehaviour
{
    public static BoardRiskManager Instance { get; private set; }

    // cellId (hex id) -> single risk (null if empty)
    private Dictionary<int, Risk> cellRisks = new Dictionary<int, Risk>();

    // All hex tiles indexed by id for quick lookup
    private Dictionary<int, HexTile> hexTilesById = new Dictionary<int, HexTile>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // Build hex tile registry
        HexTile[] allTiles = FindObjectsOfType<HexTile>();
        foreach (HexTile tile in allTiles)
        {
            hexTilesById[tile.id] = tile;
            cellRisks[tile.id] = null; // All cells start empty
        }

        Debug.Log($"[BoardRiskManager] Initialized with {hexTilesById.Count} hex tiles");
    }

    /// <summary>
    /// Adds a risk to a specific cell (replaces any existing risk)
    /// </summary>
    public void AddRiskToCell(int hexId, Risk risk)
    {
        if (risk == null)
        {
            Debug.LogWarning("[BoardRiskManager] Attempted to add null risk!");
            return;
        }

        if (!hexTilesById.ContainsKey(hexId))
        {
            Debug.LogWarning($"[BoardRiskManager] Hex ID {hexId} does not exist!");
            return;
        }

        // Set single risk (replaces any existing)
        cellRisks[hexId] = risk;

        // Update tile
        hexTilesById[hexId].SetRisk(risk);

        // Update display with animation
        BoardCellRiskDisplay display = hexTilesById[hexId].GetComponent<BoardCellRiskDisplay>();
        if (display != null)
        {
            display.UpdateRiskDisplayWithAnimation();
        }

        Debug.Log($"[BoardRiskManager] Risk '{risk.riskName}' set on cell {hexId}");
    }

    /// <summary>
    /// Removes the risk from a specific cell
    /// </summary>
    public void RemoveRiskFromCell(int hexId)
    {
        if (!hexTilesById.ContainsKey(hexId))
            return;

        cellRisks[hexId] = null;
        hexTilesById[hexId].ClearRisk();

        // Update display
        BoardCellRiskDisplay display = hexTilesById[hexId].GetComponent<BoardCellRiskDisplay>();
        if (display != null)
        {
            display.UpdateRiskDisplay();
        }

        Debug.Log($"[BoardRiskManager] Cell {hexId} cleared");
    }

    /// <summary>
    /// Gets the risk on a specific cell (null if empty)
    /// </summary>
    public Risk GetRiskOnCell(int hexId)
    {
        if (!cellRisks.ContainsKey(hexId))
            return null;

        return cellRisks[hexId];
    }

    /// <summary>
    /// Gets the cell ID containing a specific risk (returns -1 if not found)
    /// </summary>
    public int GetCellWithRisk(Risk risk)
    {
        foreach (var kvp in cellRisks)
        {
            if (kvp.Value == risk)
                return kvp.Key;
        }

        return -1;
    }

    /// <summary>
    /// Gets all cells that have risks
    /// </summary>
    public List<int> GetCellsWithAnyRisk()
    {
        return cellRisks.Where(kvp => kvp.Value != null)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    /// <summary>
    /// Gets all empty cells (cells without risks)
    /// </summary>
    public List<int> GetEmptyCells()
    {
        return cellRisks.Where(kvp => kvp.Value == null)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    /// <summary>
    /// Gets a random empty cell ID (returns -1 if no empty cells)
    /// </summary>
    public int GetRandomEmptyCellId()
    {
        List<int> emptyCells = GetEmptyCells();
        if (emptyCells.Count == 0)
            return -1;

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }

    /// <summary>
    /// Gets multiple random empty cells
    /// </summary>
    public List<int> GetRandomEmptyCells(int count)
    {
        List<int> emptyCells = GetEmptyCells();
        List<int> selected = new List<int>();

        for (int i = 0; i < count && emptyCells.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, emptyCells.Count);
            selected.Add(emptyCells[randomIndex]);
            emptyCells.RemoveAt(randomIndex);
        }

        return selected;
    }

    /// <summary>
    /// Gets the HexTile component for a given cell ID
    /// </summary>
    public HexTile GetHexTile(int hexId)
    {
        if (hexTilesById.ContainsKey(hexId))
            return hexTilesById[hexId];

        return null;
    }

    /// <summary>
    /// Gets a random cell ID (useful for risk placement)
    /// </summary>
    public int GetRandomCellId()
    {
        List<int> allIds = hexTilesById.Keys.ToList();
        if (allIds.Count == 0)
            return -1;

        return allIds[Random.Range(0, allIds.Count)];
    }

    /// <summary>
    /// Gets random cells (useful for risk placement)
    /// </summary>
    public List<int> GetRandomCells(int count)
    {
        List<int> allIds = hexTilesById.Keys.ToList();
        List<int> selected = new List<int>();

        for (int i = 0; i < count && allIds.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, allIds.Count);
            selected.Add(allIds[randomIndex]);
            allIds.RemoveAt(randomIndex);
        }

        return selected;
    }

    /// <summary>
    /// Clears all risks from all cells
    /// </summary>
    public void ClearAllCells()
    {
        foreach (int hexId in hexTilesById.Keys)
        {
            RemoveRiskFromCell(hexId);
        }

        Debug.Log("[BoardRiskManager] All cells cleared");
    }

    public void AddRandomRiskToRandomCell()
    {
        if (RiskLibrary.Instance == null || RiskLibrary.Instance.allRisks.Count == 0)
        {
            Debug.LogWarning("[BoardRiskManager] No risks available in RiskLibrary!");
            return;
        }

        int randomCellId = GetRandomEmptyCellId();
        if (randomCellId == -1)
        {
            Debug.LogWarning("[BoardRiskManager] No empty cells available!");
            return;
        }
        Risk randomRisk = RiskLibrary.Instance.allRisks[Random.Range(0, RiskLibrary.Instance.allRisks.Count)];
        AddRiskToCell(randomCellId, randomRisk);
    }
}
