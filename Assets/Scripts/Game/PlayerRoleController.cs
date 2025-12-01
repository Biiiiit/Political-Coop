using UnityEngine;
using Unity.Netcode;

public class PlayerRoleController : NetworkBehaviour
{
    public static PlayerRoleController LocalInstance { get; private set; }

    // This is the role of THIS client (this tablet)
    public Role Role { get; private set; } = Role.None;

    private void Awake()
    {
        // This may run on host + clients; only one instance is "local" per client
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
    }

    // Called from GameManager via ClientRpc when server assigns a role
    public void SetRole(Role role)
    {
        if (!IsOwner) return;

        Role = role;
        Debug.Log($"[Client {OwnerClientId}] Assigned role: {Role}");
        // Later this will drive what the tablet sees.
    }
}