using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    public List<Disaster> allDisasters;

    public Disaster CalculateDisaster(List<Risk> triggeredRisks)
    {
        HashSet<string> activeTags = new HashSet<string>(
            triggeredRisks.SelectMany(r => r.disasterTags)
        );

        List<Disaster> possible = allDisasters
            .Where(d => d.requiredRiskTags.Any(tag => activeTags.Contains(tag)))
            .ToList();

        foreach (Disaster disaster in possible)
        {
            if (Random.value < disaster.baseChance)
                return disaster;
        }

        return null;
    }
}
