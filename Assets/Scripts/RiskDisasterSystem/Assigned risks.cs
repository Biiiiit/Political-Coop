using System.Collections.Generic;
using UnityEngine;

public class Assignedrisks : MonoBehaviour
{
    [Header("Functional Card Data")]
    [Tooltip("Risks this card adds when played")]
    public List<Risk> assignedRisks = new();

    public IReadOnlyList<Risk> GetRisks()
    {
        return assignedRisks;
    }
}
