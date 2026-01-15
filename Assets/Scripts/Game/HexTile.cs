using UnityEngine;

public class HexTile : MonoBehaviour
{
    public int id;

    [Header("Height")]
    public float height = 0.2f;
    public float baseHeight = 0.2f;

    [Header("Risks")]
    private Risk activeRisk = null;

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

    /// <summary>
    /// Sets a risk on this hex tile (only one per tile)
    /// </summary>
    public void SetRisk(Risk risk)
    {
        if (activeRisk != null && activeRisk == risk)
        {
            Debug.LogWarning($"[HexTile {id}] Risk '{risk.riskName}' is already on this tile!");
            return;
        }

        if (activeRisk != null)
        {
            Debug.Log($"[HexTile {id}] Replacing risk '{activeRisk.riskName}' with '{risk?.riskName}'");
        }

        activeRisk = risk;
        if (risk != null)
        {
            Debug.Log($"[HexTile {id}] Risk '{risk.riskName}' set");
        }
    }

    /// <summary>
    /// Removes the risk from this tile
    /// </summary>
    public void ClearRisk()
    {
        if (activeRisk != null)
        {
            Debug.Log($"[HexTile {id}] Risk '{activeRisk.riskName}' cleared");
        }
        activeRisk = null;
    }

    /// <summary>
    /// Gets the current risk on this tile (or null if none)
    /// </summary>
    public Risk GetRisk()
    {
        return activeRisk;
    }

    /// <summary>
    /// Checks if this tile has a risk
    /// </summary>
    public bool HasRisk()
    {
        return activeRisk != null;
    }
}
