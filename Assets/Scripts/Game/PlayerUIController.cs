using UnityEngine;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [Header("Text fields")]
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text infoText;

    private void Start()
    {
        if (roleText != null) roleText.text = "Role: (not assigned)";
        if (resourceText != null) resourceText.text = "Resource: 0";
        if (infoText != null) infoText.text = "Waiting for role...";
    }

    public void SetRole(Role role)
    {
        if (roleText != null)
        {
            roleText.text = $"Role: {role}";
        }

        if (infoText != null)
        {
            infoText.text = "Press Play Card when allowed.";
        }
    }

    public void UpdateSector(Role role, int resourceLevel)
    {
        if (resourceText != null)
        {
            resourceText.text = $"Resource ({role}): {resourceLevel}";
        }
    }

    // Called by PlayCardButton
    public void OnPlayCardButtonClicked()
    {
        if (PlayerRoleController.LocalInstance == null)
        {
            Debug.LogWarning("[PlayerUI] No local player to send PlayCard to.");
            return;
        }

        PlayerRoleController.LocalInstance.RequestPlayCard("FAKE_CARD_UI");
    }
}
