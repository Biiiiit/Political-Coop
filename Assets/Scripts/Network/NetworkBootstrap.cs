using UnityEngine;
using Unity.Netcode;

public class NetworkBootstrap : MonoBehaviour
{
    // Set this in the Inspector before building:
    public bool StartAsHost = true; // host = board, client = tablet

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
