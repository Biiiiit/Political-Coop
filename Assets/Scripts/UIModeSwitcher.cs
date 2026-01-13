using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Manages display mode switching between shared board view and player tablet view.
/// Automatically routes displays based on host/client status and provides debug keys for testing.
/// </summary>
public class UIModeSwitcher : NetworkBehaviour
{
    [Header("Canvas References")]
    [Tooltip("Canvas shown on shared screen (Game/Board view)")]
    [SerializeField] private GameObject boardCanvasRoot;
    
    [Tooltip("Canvas shown on player tablets")]
    [SerializeField] private GameObject playerCanvasRoot;

    [Header("Debug Settings")]
    [Tooltip("Enable G/T keys for testing different views in editor")]
    [SerializeField] private bool enableDebugKeys = true;

    public override void OnNetworkSpawn()
    {
        // Automatically set mode based on whether this device is host or client
        if (IsServer)
        {
            SetBoardMode();
        }
        else
        {
            SetPlayerMode();
        }
    }

    private void Update()
    {
        // Debug keys for testing in Unity Editor
        if (!enableDebugKeys) return;

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("[UIModeSwitcher] G key pressed - Switching to Board mode");
            SetBoardMode();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[UIModeSwitcher] T key pressed - Switching to Tablet mode");
            SetPlayerMode();
        }
    }

    /// <summary>
    /// Activates board/shared screen mode (for TV/projector display)
    /// </summary>
    private void SetBoardMode()
    {
        Debug.Log("[UIModeSwitcher] Activating Board mode (shared screen)");

        if (boardCanvasRoot != null)
            boardCanvasRoot.SetActive(true);

        if (playerCanvasRoot != null)
            playerCanvasRoot.SetActive(false);
    }

    /// <summary>
    /// Activates player tablet mode (for individual player screens)
    /// </summary>
    private void SetPlayerMode()
    {
        Debug.Log("[UIModeSwitcher] Activating Player mode (tablet)");

        if (boardCanvasRoot != null)
            boardCanvasRoot.SetActive(false);

        if (playerCanvasRoot != null)
            playerCanvasRoot.SetActive(true);
    }
}
