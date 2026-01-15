using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewProjectCard", menuName = "Game/Project Card")]
public class ProjectCard : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    public Sprite backgroundImage;
    public Sprite playerImage;

    [Header("Associated Risks")]
    public List<Risk> addedRisks = new List<Risk>();
}
