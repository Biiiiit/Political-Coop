using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DiscussionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Image timerFillImage;

    [Header("Card Anchors")]
    [SerializeField] private Transform mainCardAnchor;
    [SerializeField] private Transform upcomingCardAnchor;

    [Header("Settings")]
    [SerializeField] private float discussionDuration = 15f;

    private Coroutine timerRoutine;

    public delegate void DiscussionFinished();
    public event DiscussionFinished OnDiscussionFinished;

    void Start()
    {
        Debug.Log("DiscussionManager Start() called");

        gameObject.SetActive(false);

        if (headerText != null)
            headerText.text = "Discussion Time!";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDiscussion(null, null);
        }
    }

    public void StartDiscussion(GameObject currentCardPrefab, GameObject upcomingCardPrefab)
    {
        Debug.Log("StartDiscussion CALLED");

        gameObject.SetActive(true);

        ClearChildren(mainCardAnchor);
        ClearChildren(upcomingCardAnchor);

        if (currentCardPrefab != null && mainCardAnchor != null)
        {
            GameObject mainCard = Instantiate(currentCardPrefab, mainCardAnchor);
            mainCard.transform.localScale = Vector3.one * 1.5f;
        }

        if (upcomingCardPrefab != null && upcomingCardAnchor != null)
        {
            GameObject upcomingCard = Instantiate(upcomingCardPrefab, upcomingCardAnchor);
            upcomingCard.transform.localScale = Vector3.one * 0.6f;
        }

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(RunTimer());
    }

    private IEnumerator RunTimer()
    {
        Debug.Log("Timer coroutine started");

        float timeLeft = discussionDuration;

        if (timerFillImage != null)
            timerFillImage.fillAmount = 1f;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            if (timerFillImage != null)
                timerFillImage.fillAmount = timeLeft / discussionDuration;

            yield return null;
        }

        if (timerFillImage != null)
            timerFillImage.fillAmount = 0f;

        Debug.Log("Discussion timer finished");

        OnDiscussionFinished?.Invoke();

        gameObject.SetActive(false);
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
