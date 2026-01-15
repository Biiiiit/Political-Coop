using UnityEngine;

public class CardRegistration : MonoBehaviour
{
    private ProjectCardUI projectCardUI;
    private Assignedrisks assignedrisks;

    void Start()
    {
        // Get components from this card
        projectCardUI = GetComponent<ProjectCardUI>();
        assignedrisks = GetComponent<Assignedrisks>();

        if (projectCardUI == null || assignedrisks == null)
        {
            Debug.LogError("[CardRegistration] Card prefab missing ProjectCardUI or Assignedrisks component!");
            return;
        }

        // Wait for CardCreationController to be initialized
        if (CardCreationController.Instance == null)
        {
            Debug.LogWarning("[CardRegistration] CardCreationController.Instance not found! Make sure CardCreationController exists in the scene.");
            return;
        }

        // Register this card with the controller
        CardCreationController.Instance.RegisterCard(projectCardUI, assignedrisks);
        Debug.Log("[CardRegistration] Card registered with controller");
    }
}
