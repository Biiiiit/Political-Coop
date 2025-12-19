using UnityEngine;

public class HexTile : MonoBehaviour
{
    public int id;

    [Header("Height")]
    public float height = 0.2f;
    public float baseHeight = 0.2f;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        ApplyHeight();
    }

    public void ApplyHeight()
    {
        Vector3 scale = transform.localScale;
        scale.y = height;
        transform.localScale = scale;

        Vector3 pos = transform.position;
        pos.y = height * 0.5f;
        transform.position = pos;
    }

    public void SetColor(Color color)
    {
        rend.material.color = color;
    }
}
