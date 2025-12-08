using UnityEngine;

[CreateAssetMenu(fileName = "NewDisaster", menuName = "Game/Disaster")]
public class Disaster : ScriptableObject
{
    public string disasterName;
    [TextArea] public string description;

    [Header("Trigger Conditions")]
    public string[] requiredRiskTags;

    [Header("Outcome")]
    public float baseChance = 0.1f; // Chance once matching risks exist
    public float severityMultiplier = 1.5f; // Amplifies negative points
}
