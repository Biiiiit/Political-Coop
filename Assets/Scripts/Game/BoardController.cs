using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class BoardController : NetworkBehaviour
{
    public static BoardController LocalInstance { get; private set; }

    [SerializeField] private BoardUIController boardUI;

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
            Debug.Log("[BoardController] Not server, disabling.");
            enabled = false;
            return;
        }

        Debug.Log("[BoardController] Active on host (board screen).");

        // Try to auto-find UI if not set
        if (boardUI == null)
        {
            boardUI = FindFirstObjectByType<BoardUIController>();
        }
    }

    public void OnBoardStateUpdated(int turnNumber, int crisisLevel, Phase phase)
    {
        Debug.Log($"[BoardController] Turn {turnNumber}, Crisis {crisisLevel}, Phase {phase}");

        if (boardUI != null)
        {
            boardUI.UpdateBoard(turnNumber, crisisLevel, phase);
            boardUI.AddLogMessage($"Board updated: Turn {turnNumber}, Phase {phase}, Crisis {crisisLevel}");
        }
        
        // Notify GameFlowManager of phase change
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnPhaseChanged(phase);
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        // Debug: press N on board to go to next phase
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            Debug.Log("[BoardController] N key detected!");
            RequestNextPhase();
        }
    }

    public void RequestNextPhase()
    {
        Debug.Log("[BoardController] RequestNextPhase called!");
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("[BoardController] GameManager.Instance is null!");
            return;
        }
        
        Debug.Log("[BoardController] Requesting next phase via GameManager.");
        GameManager.Instance.NextPhaseServerRpc();
    }

    // Hook this to the NextPhaseButton OnClick
    public void OnNextPhaseButtonClicked()
    {
        Debug.Log("[BoardController] OnNextPhaseButtonClicked called!");
        
        if (!IsServer) 
        {
            Debug.LogWarning("[BoardController] Not server, cannot advance phase!");
            return;
        }
        
        RequestNextPhase();
    }
}
