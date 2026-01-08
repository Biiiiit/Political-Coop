using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<HexTile> tiles = new List<HexTile>();

    void Awake()
    {
        tiles.AddRange(FindObjectsOfType<HexTile>());
    }

    public void GiveRandomTileRed()
    {
        if (tiles.Count == 0) return;

        int randomIndex = Random.Range(0, tiles.Count);
        tiles[randomIndex].SetColor(Color.red);
    }

    void Start()
    {
        foreach (HexTile tile in tiles)
        {
            tile.height = Random.Range(0f, 0.5f);
            tile.ApplyHeight();
        }
    }

}
