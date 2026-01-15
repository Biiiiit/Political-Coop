using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI remainingText;
    public Image icon;

    private Effect boundEffect;

    public void SetText(
        string name,
        string description,
        Effect effect,
        Sprite continuous,
        Sprite limited,
        Sprite singleUse)
    {
        boundEffect = effect;

        nameText.text = name;
        descriptionText.text = description;

        switch (effect.type)
        {
            case EffectType.Continuous:
                icon.sprite = continuous;
                remainingText.text = "∞";
                break;

            case EffectType.Limited:
                icon.sprite = limited;
                UpdateRemaining();
                break;

            case EffectType.SingleUse:
                icon.sprite = singleUse;
                remainingText.text = "1";
                break;
        }
    }

    public void UpdateRemaining()
    {
        if (boundEffect == null) return;

        if (boundEffect.type == EffectType.Limited)
        {
            remainingText.text = boundEffect.turns > 0
                ? boundEffect.remainingTurns.ToString()
                : boundEffect.remainingUses.ToString();
        }
        else if (boundEffect.type == EffectType.SingleUse)
        {
            remainingText.text = "0";
        }
    }
}
