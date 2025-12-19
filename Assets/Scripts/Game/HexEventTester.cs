using UnityEngine;

public class HexEventTester : MonoBehaviour
{
    public HexBoardGenerator board;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int randomIndex = Random.Range(0, board.hexTiles.Count);
            board.hexTiles[randomIndex].SetColor(Color.blue);
        }
    }
}
