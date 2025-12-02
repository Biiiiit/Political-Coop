using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerRoleController : NetworkBehaviour
{
    public static PlayerRoleController LocalInstance { get; private set; }

    [SerializeField] private PlayerUIController playerUI;

    public Role Role { get; private set; } = Role.None;
    public int ResourceLevel { get; private set; }

    private Phase currentPhase = Phase.Lobby;

    private void Awake()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        LocalInstance = this;
        Debug.Log($"[Client {OwnerClientId}] PlayerRoleController spawned.");

        if (playerUI == null)
        {
            playerUI = FindFirstObjectByType<PlayerUIController>();
        }
    }

    public void SetRole(Role role)
    {
        if (!IsOwner) return;

        Role = role;
        Debug.Log($"[Client {OwnerClientId}] Assigned role: {Role}");

        if (playerUI != null)
        {
            playerUI.SetRole(role);
        }
    }

    public void OnSectorStateUpdated(Role role, int resourceLevel)
    {
        if (!IsOwner) return;

        ResourceLevel = resourceLevel;
        Debug.Log($"[Client {OwnerClientId}] Sector update: role={role}, resource={resourceLevel}");

        if (playerUI != null)
        {
            playerUI.UpdateSector(role, resourceLevel);
        }
    }

    public void OnBoardStateUpdated(int turnNumber, int crisisLevel, Phase phase)
    {
        if (!IsOwner) return;

        currentPhase = phase;
        Debug.Log($"[Client {OwnerClientId}] Board state for player: Turn {turnNumber}, Crisis {crisisLevel}, Phase {phase}");

        if (playerUI != null)
        {
            playerUI.UpdateBoardState(turnNumber, crisisLevel, phase);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Debug key still works, but respects phase.
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            RequestPlayCard("FAKE_CARD_P_KEY");
        }
    }

    // Called from UI or from debug key
    public void RequestPlayCard(string cardId)
    {
        if (currentPhase != Phase.Play)
        {
            Debug.Log($"[Client {OwnerClientId}] Not in Play phase, cannot play card.");
            return;
        }

        Debug.Log($"[Client {OwnerClientId}] Requesting to play card {cardId}");
        GameManager.Instance.PlayCardServerRpc(OwnerClientId, cardId);
    }
}
