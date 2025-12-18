using UnityEngine;
using UnityEngine.UI;

public class RiskIconUI : MonoBehaviour
{
    public Image iconImage;

    public void SetRisk(Risk risk)
    {
        if (iconImage != null && risk != null)
            iconImage.sprite = risk.icon;
    }
}

