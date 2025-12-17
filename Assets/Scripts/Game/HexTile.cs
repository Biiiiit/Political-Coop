using UnityEngine;

public class HexTile : MonoBehaviour
{
    public int id;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }
}
