using UnityEngine;

public class PlayedCardStore : MonoBehaviour
{
    public static PlayedCardStore Instance { get; private set; }

    [Header("Debug / Inspector Reference")]
    public GameObject PlayedCardPrefab; // now visible in inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StoreCard(GameObject cardPrefab)
    {
        PlayedCardPrefab = cardPrefab;
    }

    public void Clear()
    {
        PlayedCardPrefab = null;
    }
}
