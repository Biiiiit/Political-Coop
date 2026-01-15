using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<HexTile> tiles = new List<HexTile>();

    [Header("Initial Colors by ID")]
    public List<int> redIds = new List<int>();
    public List<int> yellowIds = new List<int>();
    public List<int> blueIds = new List<int>();
    public List<int> greenIds = new List<int>();

    void Awake()
    {
        tiles.AddRange(FindObjectsOfType<HexTile>());
    }

    void Start()
    {
        ApplyInitialColors();
        ApplyRandomHeights();
    }

    void ApplyInitialColors()
    {
        foreach (HexTile tile in tiles)
        {
            if (redIds.Contains(tile.id))
                tile.SetColor(Color.red);
            else if (yellowIds.Contains(tile.id))
                tile.SetColor(Color.yellow);
            else if (blueIds.Contains(tile.id))
                tile.SetColor(Color.blue);
            else if (greenIds.Contains(tile.id))
                tile.SetColor(Color.green);
        }
    }

    void ApplyRandomHeights()
    {
        foreach (HexTile tile in tiles)
        {
            tile.height = Random.Range(0.2f, 0.6f);
            tile.ApplyHeight();
        }
    }
}
