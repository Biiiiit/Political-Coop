using UnityEngine;

public class HexTile : MonoBehaviour
{
    public int id;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetColor(Color color)
    {
        rend.material.color = color;
    }
}
