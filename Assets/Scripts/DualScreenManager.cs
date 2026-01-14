using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Manages dual-screen display: Tablet (client) vs Shared Screen (host).
/// Each scene should have both a TabletCanvas and SharedCanvas.
/// This manager activates the appropriate canvas based on whether this is host or client.
/// Note: Does NOT require NetworkObject component - detects network state via NetworkManager.
/// </summary>
public class DualScreenManager : MonoBehaviour
{
    public static DualScreenManager Instance { get; private set; }

    [Header("Canvas References")]
    [SerializeField] private GameObject tabletCanvas;
    [SerializeField] private GameObject sharedCanvas;

    [Header("Screen Type")]
    [SerializeField] private ScreenType currentScreenType = ScreenType.GameLibrary;

    [Header("Testing")]
    [Tooltip("Force shared screen mode when not networked (for testing)")]
    [SerializeField] private bool forceSharedScreenInOfflineMode = false;

    public enum ScreenType
    {
        Screens,        // Tablet: player lobby UI, Shared: waiting for players
        GameLibrary,    // Tablet: topic selection, Shared: waiting
        CardLibrary,    // Tablet: card selection, Shared: waiting
        GameScreen      // Tablet: gameplay, Shared: full board display
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Check if we're in a networked session
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            Debug.Log("[DualScreenManager] Networked session detected");
            ConfigureScreens();
        }
        else
        {
            Debug.Log("[DualScreenManager] Not networked, using offline configuration");
            ConfigureScreensNonNetworked();
        }
    }

    private void ConfigureScreens()
    {
        bool isHost = NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
        
        Debug.Log($"[DualScreenManager] ConfigureScreens called - IsServer: {isHost}, NetworkManager.IsListening: {NetworkManager.Singleton?.IsListening}");
        
        if (isHost)
        {
            // Host = Shared Screen
            ShowSharedScreen();
        }
        else
        {
            // Client = Tablet
            ShowTabletScreen();
        }

        Debug.Log($"[DualScreenManager] Configured for {(isHost ? "SHARED SCREEN (Host)" : "TABLET (Client)")} - Screen: {currentScreenType}");
    }

    private void ConfigureScreensNonNetworked()
    {
        // Fallback for testing without network
        if (forceSharedScreenInOfflineMode)
        {
            ShowSharedScreen();
            Debug.Log($"[DualScreenManager] Non-networked mode - FORCED to SHARED SCREEN - Screen: {currentScreenType}");
        }
        else
        {
            ShowTabletScreen();
            Debug.Log($"[DualScreenManager] Non-networked mode - defaulting to TABLET - Screen: {currentScreenType}");
        }
    }

    private void ShowTabletScreen()
    {
        if (tabletCanvas != null) tabletCanvas.SetActive(true);
        if (sharedCanvas != null) sharedCanvas.SetActive(false);
    }

    private void ShowSharedScreen()
    {
        if (tabletCanvas != null) tabletCanvas.SetActive(false);
        if (sharedCanvas != null) sharedCanvas.SetActive(true);
    }

    /// <summary>
    /// Manually set which canvas to show (for testing or special cases)
    /// </summary>
    public void SetScreenMode(bool showTablet)
    {
        if (showTablet)
            ShowTabletScreen();
        else
            ShowSharedScreen();
    }

    /// <summary>
    /// Get the current screen type
    /// </summary>
    public ScreenType GetCurrentScreenType()
    {
        return currentScreenType;
    }

    /// <summary>
    /// Check if this instance is showing the tablet view
    /// </summary>
    public bool IsTabletMode()
    {
        return tabletCanvas != null && tabletCanvas.activeSelf;
    }

    /// <summary>
    /// Check if this instance is showing the shared screen view
    /// </summary>
    public bool IsSharedMode()
    {
        return sharedCanvas != null && sharedCanvas.activeSelf;
    }

    /// <summary>
    /// Check if currently running as host/server
    /// </summary>
    public bool IsHost()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
    }
}
