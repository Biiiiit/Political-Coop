using UnityEngine;
using Unity.Netcode;

public class NetworkBootstrap : MonoBehaviour
{
    // - Host (board) build: true
    // - Tablet/client build: false
    public bool StartAsHost = true;

    private void Start()
    {
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
