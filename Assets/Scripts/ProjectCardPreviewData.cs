using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectCardPreviewData
{
    public string cardName;
    public Sprite backgroundImage;
    public Sprite playerImage;
    public List<Risk> addedRisks = new();
}
