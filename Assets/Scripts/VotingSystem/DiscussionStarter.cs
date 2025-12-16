using UnityEngine;

public class DiscussionStarter : MonoBehaviour
{
    [SerializeField] private DiscussionManager discussionManager;

    [Header("Test Cards")]
    [SerializeField] private GameObject currentCardPrefab;
    [SerializeField] private GameObject upcomingCardPrefab;

    void Start()
    {
        Debug.Log("DiscussionStarter Start()");

        if (discussionManager == null)
        {
            Debug.LogError("DiscussionManager reference missing");
            return;
        }

        discussionManager.StartDiscussion(currentCardPrefab, upcomingCardPrefab);
    }
}
