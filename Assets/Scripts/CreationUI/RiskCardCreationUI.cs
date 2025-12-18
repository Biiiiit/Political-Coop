using UnityEngine;
using UnityEngine.UI;

public class RiskCardCreationUI : MonoBehaviour
{
    public Image icon;

    public void SetRisk(Risk risk)
    {
        icon.sprite = risk.icon;
        icon.color = risk.color;
    }
}
