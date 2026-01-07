using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPointsUI : MonoBehaviour
{
    [Header("Points")]
    [Range(0, 10)]
    public int currentPoints = 4;

    public Image[] pointKnobs;

    [Header("Animation")]
    public float animationDuration = 0.15f;
    public float gainScale = 1.3f;
    public float loseScale = 0.6f;

    void Start()
    {
        UpdateVisualState();
    }

    public void AddPoint()
    {
        if (currentPoints >= pointKnobs.Length) return;

        Image point = pointKnobs[currentPoints];

        // Start from 0 scale
        point.transform.localScale = Vector3.zero;
        point.enabled = true;

        // Animate from 0 → gainScale → 1
        StartCoroutine(GrowRoutine(point));
        currentPoints++;
    }

    private IEnumerator GrowRoutine(Image point)
    {
        Vector3 target = Vector3.one * gainScale;
        float time = 0f;

        // Grow from 0 → gainScale
        while (time < animationDuration)
        {
            point.transform.localScale = Vector3.Lerp(Vector3.zero, target, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Snap to gainScale
        point.transform.localScale = target;

        // Shrink back to 1
        time = 0f;
        Vector3 start = target;
        Vector3 end = Vector3.one;

        while (time < animationDuration)
        {
            point.transform.localScale = Vector3.Lerp(start, end, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        point.transform.localScale = end;
    }

    public void RemovePoint()
    {
        if (currentPoints <= 0) return;

        currentPoints--;
        Image point = pointKnobs[currentPoints];

        StartCoroutine(ShrinkAndDisable(point));
    }

    private IEnumerator ShrinkAndDisable(Image point)
    {
        Vector3 start = Vector3.one;
        Vector3 target = Vector3.one * loseScale;
        float time = 0f;

        // Shrink from 1 → loseScale
        while (time < animationDuration)
        {
            point.transform.localScale = Vector3.Lerp(start, target, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        point.transform.localScale = target;

        // Shrink from loseScale → 0
        time = 0f;
        start = target;
        target = Vector3.zero;

        while (time < animationDuration)
        {
            point.transform.localScale = Vector3.Lerp(start, target, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        point.transform.localScale = Vector3.zero;
        point.enabled = false;
    }


    void UpdateVisualState()
    {
        for (int i = 0; i < pointKnobs.Length; i++)
        {
            pointKnobs[i].enabled = i < currentPoints;
            pointKnobs[i].transform.localScale = Vector3.one;
        }
    }

    void Animate(Image point, float targetScale, System.Action onComplete = null)
    {
        StopCoroutine(nameof(ScaleRoutine));
        StartCoroutine(ScaleRoutine(point, targetScale, onComplete));
    }

    IEnumerator ScaleRoutine(Image point, float targetScale, System.Action onComplete)
    {
        Vector3 start = Vector3.one;
        Vector3 target = Vector3.one * targetScale;
        float time = 0f;

        while (time < animationDuration)
        {
            point.transform.localScale =
                Vector3.Lerp(start, target, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        point.transform.localScale = Vector3.one;
        onComplete?.Invoke();
    }
}
