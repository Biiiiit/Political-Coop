using UnityEngine;
using Unity.Netcode;

public class UIModeSwitcher : NetworkBehaviour
{
    [Header("Assign these in the Inspector")]
    [SerializeField] private GameObject boardCanvasRoot;
    [SerializeField] private GameObject playerCanvasRoot;

    public override void OnNetworkSpawn()
    {
        // Host = IsServer == true (in your setup, board screen starts as Host)
        if (IsServer)
        {
            SetBoardMode();
        }
        else
        {
            SetPlayerMode();
        }
    }

    private void SetBoardMode()
    {
        Debug.Log("[UIModeSwitcher] Board mode (host).");

        if (boardCanvasRoot != null)
            boardCanvasRoot.SetActive(true);

        if (playerCanvasRoot != null)
            playerCanvasRoot.SetActive(false);
    }

    private void SetPlayerMode()
    {
        Debug.Log("[UIModeSwitcher] Player mode (client/tablet).");

        if (boardCanvasRoot != null)
            boardCanvasRoot.SetActive(false);

        if (playerCanvasRoot != null)
            playerCanvasRoot.SetActive(true);
    }
}
