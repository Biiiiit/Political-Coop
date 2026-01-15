using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum EffectType { Continuous, Limited, SingleUse }

[System.Serializable]
public class Effect
{
    public string effectName;
    public string effectDescription;
    public EffectType type;

    public int turns = 0;
    public int totalUses = 0;

    [HideInInspector] public int remainingTurns;
    [HideInInspector] public int remainingUses;

    public Effect(string name, string desc, EffectType type, int turns = 0, int totalUses = 0)
    {
        effectName = name;
        effectDescription = desc;
        this.type = type;
        this.turns = turns;
        this.totalUses = totalUses;
        remainingTurns = turns;
        remainingUses = totalUses;
    }

    public bool IsActive()
    {
        return type == EffectType.Continuous ||
               remainingTurns > 0 ||
               remainingUses > 0;
    }

    public void TickTurn()
    {
        if (type == EffectType.Limited && remainingTurns > 0) remainingTurns--;
    }

    public void Use()
    {
        if (type == EffectType.Limited && remainingUses > 0) remainingUses--;
        if (type == EffectType.SingleUse) remainingUses = 0;
    }
}

public class DisplayEffectManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform sidePanelParent;
    public GameObject effectItemPrefab;
    public RectTransform centerPanel; // This is where effect temporarily moves
    public float zoomScale = 1.5f;
    public float animationDuration = 0.5f;

    [Header("Effect Type Sprites")]
    public Sprite continuousSprite;
    public Sprite limitedSprite;
    public Sprite singleUseSprite;

    private Dictionary<string, GameObject> sidePanelUI = new Dictionary<string, GameObject>();
    private Dictionary<string, Effect> activeEffects = new Dictionary<string, Effect>();

    private List<Effect> randomEffectPool = new List<Effect>()
    {
        new Effect("Extra Point", "Gain 1 extra point", EffectType.Continuous),
        new Effect("Shield", "Prevent risk for 2 turns", EffectType.Limited, turns: 2),
        new Effect("Instant Bonus", "Get 3 points immediately", EffectType.SingleUse)
    };

    #region Public Methods

    // Add effect to side panel only
    public void AddEffect(Effect effect)
    {
        if (activeEffects.ContainsKey(effect.effectName))
        {
            activeEffects[effect.effectName] = effect;
        }
        else
        {
            activeEffects.Add(effect.effectName, effect);

            GameObject item = Instantiate(effectItemPrefab, sidePanelParent);
            item.GetComponent<EffectItem>().SetText(effect.effectName, effect.effectDescription, effect,
                continuousSprite, limitedSprite, singleUseSprite);

            sidePanelUI.Add(effect.effectName, item);
        }

        UpdateSidePanelVisibility();
    }

    // Pick and add a random effect
    public void AddRandomEffect()
    {
        if (randomEffectPool.Count == 0) return;

        int index = Random.Range(0, randomEffectPool.Count);
        Effect chosen = randomEffectPool[index];

        AddEffect(chosen);
        randomEffectPool.RemoveAt(index);
    }

    // Trigger an effect: animate it while temporarily moving to main panel
    public void TriggerEffect(string effectName)
    {
        if (!activeEffects.TryGetValue(effectName, out Effect effect)) return;
        if (!sidePanelUI.TryGetValue(effectName, out GameObject sideItem)) return;

        StartCoroutine(AnimateEffectFromSidePanel(effect, sideItem));
    }

    // Trigger a random effect from pool
    public void TriggerRandomEffect()
    {
        if (sidePanelUI.Count == 0) return;

        // Pick a random existing effect from the side panel
        List<string> keys = new List<string>(sidePanelUI.Keys);
        int index = Random.Range(0, keys.Count);
        string effectName = keys[index];

        if (activeEffects.TryGetValue(effectName, out Effect effect) &&
            sidePanelUI.TryGetValue(effectName, out GameObject sideItem))
        {
            StartCoroutine(AnimateEffectFromSidePanel(effect, sideItem));
        }
    }

    #endregion

    #region Animation

    private IEnumerator AnimateEffectFromSidePanel(Effect effect, GameObject effectItem)
    {
        // Show center panel
        centerPanel.gameObject.SetActive(true);

        // Move effect to center panel
        effectItem.transform.SetParent(centerPanel, worldPositionStays: false);
        effectItem.transform.localPosition = Vector3.zero;

        Vector3 originalScale = effectItem.transform.localScale;
        Vector3 zoomedScale = originalScale * zoomScale;

        // Zoom out
        float t = 0f;
        while (t < animationDuration)
        {
            effectItem.transform.localScale =
                Vector3.Lerp(originalScale, zoomedScale, t / animationDuration);
            t += Time.deltaTime;
            yield return null;
        }
        effectItem.transform.localScale = zoomedScale;

        // Zoom back in
        t = 0f;
        while (t < animationDuration)
        {
            effectItem.transform.localScale =
                Vector3.Lerp(zoomedScale, originalScale, t / animationDuration);
            t += Time.deltaTime;
            yield return null;
        }
        effectItem.transform.localScale = originalScale;

        // Apply effect logic AFTER animation
        EffectItem item = effectItem.GetComponent<EffectItem>();

        if (effect.type == EffectType.Limited)
        {
            effect.TickTurn();
            item.UpdateRemaining();
        }
        else if (effect.type == EffectType.SingleUse)
        {
            effect.Use();
            item.UpdateRemaining();
        }

        // Move back to side panel
        effectItem.transform.SetParent(sidePanelParent, worldPositionStays: false);

        // Remove if expired
        if (!effect.IsActive())
        {
            Destroy(effectItem);
            sidePanelUI.Remove(effect.effectName);
            activeEffects.Remove(effect.effectName);
        }

        // Hide center panel
        centerPanel.gameObject.SetActive(false);

        UpdateSidePanelVisibility();
    }

    #endregion

    #region Game Logic

    public void TickEffects()
    {
        List<string> toRemove = new List<string>();
        foreach (var kvp in activeEffects)
        {
            Effect e = kvp.Value;
            if (e.type == EffectType.Limited)
            {
                e.TickTurn();
                if (!e.IsActive()) toRemove.Add(kvp.Key);
            }
            else if (e.type == EffectType.SingleUse)
            {
                if (e.remainingUses <= 0) toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove)
        {
            if (sidePanelUI.TryGetValue(key, out GameObject item))
            {
                Destroy(item);
                sidePanelUI.Remove(key);
            }
            activeEffects.Remove(key);
        }

        UpdateSidePanelVisibility();
    }

    #endregion

    #region Helpers

    private void UpdateSidePanelVisibility()
    {
        sidePanelParent.gameObject.SetActive(sidePanelUI.Count > 0);
    }

    #endregion
}
