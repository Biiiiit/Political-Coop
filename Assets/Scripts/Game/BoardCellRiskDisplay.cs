using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Displays risk icon and visual indicators on hex tiles
/// Handles animation when risk is added
/// </summary>
public class BoardCellRiskDisplay : MonoBehaviour
{
    [Header("Risk Display")]
    public GameObject riskIconPrefab;

    [Header("Visual Feedback")]
    public Color riskHighlightColor = new Color(1f, 0.5f, 0.5f, 1f);
    public float riskHighlightAlpha = 0.7f;

    [Header("Animation")]
    public string riskAddedAnimationTrigger = "ShowRisk";
    public float animationDuration = 0.5f;

    [Header("Icon Sizing")]
    public float iconSize = 1f; // Size of the risk icon in world space

    private HexTile hexTile;
    private Renderer tileRenderer;
    private Color originalColor;
    private GameObject displayedRiskIcon;
    private Canvas tileCanvas;
    private Transform tileCanvasTransform;

    void Start()
    {
        hexTile = GetComponent<HexTile>();
        tileRenderer = GetComponent<Renderer>();

        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }

        // Create a world-space canvas for this tile's risk icon
        CreateTileCanvas();
    }

    private void CreateTileCanvas()
    {
        GameObject canvasObj = new GameObject("RiskIconCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, 1.2f, 0); // Position higher above the tile
        // Rotate to face the angled camera (50 degrees on X axis)
        canvasObj.transform.localRotation = Quaternion.Euler(50f, 0f, 0f);

        tileCanvas = canvasObj.AddComponent<Canvas>();
        tileCanvas.renderMode = RenderMode.WorldSpace;

        // Configure the RectTransform for world space
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1.8f, 1.8f);
        canvasRect.localScale = Vector3.one * 0.01f;

        tileCanvasTransform = canvasObj.transform;

        Debug.Log($"[BoardCellRiskDisplay] Created world-space Canvas on tile {hexTile.id}");
    }

    /// <summary>
    /// Updates the visual display based on current risk on the tile
    /// </summary>
    public void UpdateRiskDisplay()
    {
        if (hexTile == null) return;

        // Clear old icon
        if (displayedRiskIcon != null)
            Destroy(displayedRiskIcon);
        displayedRiskIcon = null;

        Risk risk = hexTile.GetRisk();
        
        // Update tile color/highlight based on risk presence
        if (risk != null)
        {
            ApplyRiskHighlight(risk);
            CreateRiskIcon(risk);
        }
        else
        {
            RestoreOriginalColor();
        }

        Debug.Log($"[BoardCellRiskDisplay] Tile {hexTile.id} updated with risk: {(risk != null ? risk.riskName : "None")}");
    }

    /// <summary>
    /// Updates display and triggers animation for new risk
    /// </summary>
    public void UpdateRiskDisplayWithAnimation()
    {
        UpdateRiskDisplay();
        TriggerRiskAddedAnimation();
    }

    private void TriggerRiskAddedAnimation()
    {
        // Trigger animation on the risk icon itself
        if (displayedRiskIcon != null)
        {
            Animator iconAnimator = displayedRiskIcon.GetComponent<Animator>();
            if (iconAnimator != null)
            {
                iconAnimator.SetTrigger(riskAddedAnimationTrigger);
                Debug.Log($"[BoardCellRiskDisplay] Animation '{riskAddedAnimationTrigger}' triggered on risk icon");
            }
            else
            {
                Debug.LogWarning($"[BoardCellRiskDisplay] No Animator component found on risk icon");
            }
        }
        else
        {
            Debug.LogWarning($"[BoardCellRiskDisplay] No risk icon to animate on tile {hexTile.id}");
        }
    }

    private void ApplyRiskHighlight(Risk risk)
    {
        if (tileRenderer == null) return;

        // Blend the current color with red based on risk severity
        float severityFactor = Mathf.Clamp01(risk.severity / 5f);
        
        Color currentColor = tileRenderer.material.color;
        Color blendedColor = Color.Lerp(currentColor, Color.red, riskHighlightAlpha * severityFactor);
        
        // Create material instance and apply color
        Material materialInstance = new Material(tileRenderer.material);
        materialInstance.color = blendedColor;
        tileRenderer.material = materialInstance;
    }

    private void RestoreOriginalColor()
    {
        if (tileRenderer == null) return;

        // Let HexTile.SetColor manage the color - don't restore here
    }

    private void CreateRiskIcon(Risk risk)
    {
        if (riskIconPrefab == null)
        {
            Debug.LogWarning("[BoardCellRiskDisplay] riskIconPrefab not assigned!");
            return;
        }

        if (tileCanvasTransform == null)
        {
            Debug.LogWarning("[BoardCellRiskDisplay] Tile canvas not created!");
            return;
        }

        // Instantiate as child of the world-space canvas
        displayedRiskIcon = Instantiate(riskIconPrefab, tileCanvasTransform);
        displayedRiskIcon.name = "RiskIcon_" + risk.riskName;

        // Position at canvas center
        RectTransform rectTransform = displayedRiskIcon.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.sizeDelta = new Vector2(iconSize * 100, iconSize * 100); // Scale based on inspector setting
        }

        // Set risk icon image if component exists
        Image iconImage = displayedRiskIcon.GetComponent<Image>();
        if (iconImage != null && risk.icon != null)
        {
            iconImage.sprite = risk.icon;
            iconImage.color = risk.color;
        }

        Debug.Log($"[BoardCellRiskDisplay] Risk icon created for '{risk.riskName}' on tile {hexTile.id}");
    }
}
