using UnityEngine;

[CreateAssetMenu(fileName = "NewRisk", menuName = "Game/Risk")]
public class Risk : ScriptableObject
{
    [Header("Basic Info")]
    public string riskName;
    [TextArea] public string description;
    public string category; // Player role: Farming (1), Industry (2), Housing (3), Nature (4)

    [Header("Risk Behavior")]
    [Range(1, 5)] public int severity = 1;
    public string[] disasterTags; // Tags linking risks to disasters if multiple apply

    [Range(0f, 1f)] public float baseProbability = 0.02f;
    [Range(0f, 0.2f)] public float escalationRate = 0.01f;

    [Header("Visuals")]
    public Sprite icon;
    public Color color = Color.white;
}
