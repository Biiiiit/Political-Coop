using UnityEngine;
using Unity.Netcode;

public class NetworkBootstrap : MonoBehaviour
{
    // - Host (board) build: true
    // - Tablet/client build: false
    public bool StartAsHost = true;

    private static bool started;

    private void Awake()
    {
        // Keep this bootstrap object across scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Prevent starting networking multiple times when changing scenes
        if (started)
            return;

        started = true;

        // If NetworkManager is already listening, do nothing
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            return;

        if (StartAsHost)
        {
            Debug.Log("[NetworkBootstrap] Starting as HOST (board).");
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.Log("[NetworkBootstrap] Starting as CLIENT (tablet).");
            NetworkManager.Singleton.StartClient();
        }
    }
}
