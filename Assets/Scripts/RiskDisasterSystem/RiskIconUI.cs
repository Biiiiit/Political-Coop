using UnityEngine;
using UnityEngine.UI;

public class RiskIconUI : MonoBehaviour
{
    [SerializeField] private Image contentImage;

    public void SetRisk(Risk risk)
    {
        if (risk.icon != null)
        {
            contentImage.sprite = risk.icon;
            contentImage.enabled = true;
        }
        else
        {
            contentImage.enabled = false;
        }
    }
}
