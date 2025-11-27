using UnityEngine;

public class RiskTile : MonoBehaviour
{
    public string ownerPlayerId;
    public Risk currentRisk;

    public bool IsEmpty => currentRisk == null;
}
