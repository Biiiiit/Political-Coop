using System.Collections.Generic;
using UnityEngine;

public class HexBoardGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public int radiusInHexes = 3;
    public float hexSize = 0.5f;

    public List<HexTile> hexTiles = new List<HexTile>();

    void Start()
    {
        GenerateHexCircle();
    }

    void GenerateHexCircle()
    {
        float hexWidth = hexSize * 2.0f;
        float hexHeight = Mathf.Sqrt(3.0f) * hexSize;

        int id = 0;

        for (int q = -radiusInHexes; q <= radiusInHexes; q++)
        {
            int r1 = Mathf.Max(-radiusInHexes, -q - radiusInHexes);
            int r2 = Mathf.Min(radiusInHexes, -q + radiusInHexes);

            for (int r = r1; r <= r2; r++)
            {
                float x = hexWidth * (3.0f / 4.0f) * q;
                float z = hexHeight * (r + q / 2.0f);

                Vector3 pos = new Vector3(x, 0.0f, z);
                GameObject hex = Instantiate(hexPrefab, pos, Quaternion.identity, transform);

                HexTile tile = hex.GetComponent<HexTile>();
                tile.id = id;
                hexTiles.Add(tile);

                id++;
            }
        }
    }
}
