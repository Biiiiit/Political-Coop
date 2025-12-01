using UnityEngine;
using Unity.Netcode;

public class BoardController : NetworkBehaviour
{
    public static BoardController LocalInstance { get; private set; }

    private void Awake()
    {
        if (LocalInstance != null && LocalInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        LocalInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            // Only the host should act as the board controller
            Debug.Log("[BoardController] Not server, disabling.");
            enabled = false;
            return;
        }

        Debug.Log("[BoardController] Active on host (board screen).");
    }

    public void OnBoardStateUpdated(int turnNumber, int crisisLevel)
    {
        // Called from GameManager.BroadcastBoardStateClientRpc
        Debug.Log($"[Board] Turn {turnNumber}, Crisis {crisisLevel}");
        // Later: update board UI here.
    }

    // Debug helper: lets the host advance the turn from inspector
    [ContextMenu("Advance Turn (Host Only)")]
    public void DebugAdvanceTurn()
    {
        if (!IsServer) return;
        GameManager.Instance.AdvanceTurnServerRpc();
    }
}
