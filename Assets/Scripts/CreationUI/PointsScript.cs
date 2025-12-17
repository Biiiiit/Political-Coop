using UnityEngine;
using TMPro;

public class CircleSpawnerTMP : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject positiveCirclePrefab;  // green
    public GameObject negativeCirclePrefab;  // red

    [Header("Dropdowns")]
    public TMP_Dropdown positiveDropdown;    // number of positive points
    public TMP_Dropdown negativeDropdown;    // number of negative points

    private void Start()
    {
        positiveDropdown.onValueChanged.AddListener(_ => UpdateCircles());

        UpdateCircles();
    }

    void UpdateCircles()
    {
        // Clear old circles
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Read positive number
        int pos = int.Parse(positiveDropdown.options[positiveDropdown.value].text);

        // Spawn positive circles (green)
        for (int i = 0; i < pos; i++)
            Instantiate(positiveCirclePrefab, transform);
    }
}
