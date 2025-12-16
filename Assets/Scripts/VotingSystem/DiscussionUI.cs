using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DiscussionUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Image timerFillImage;

    [Header("Cards")]
    [SerializeField] private Transform mainCardAnchor;
    [SerializeField] private Transform upcomingCardAnchor;

    [Header("Settings")]
    [SerializeField] private float discussionDuration = 15f;

    private Coroutine timerRoutine;

    void Start()
    {
        headerText.text = "Discussion Time!";
        gameObject.SetActive(false);
    }

    public void Show(GameObject currentCard, GameObject upcomingCard)
    {
        gameObject.SetActive(true);

        Clear(mainCardAnchor);
        Clear(upcomingCardAnchor);

        Instantiate(currentCard, mainCardAnchor).transform.localScale = Vector3.one * 1.5f;
        Instantiate(upcomingCard, upcomingCardAnchor).transform.localScale = Vector3.one * 0.6f;

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        float t = discussionDuration;
        timerFillImage.fillAmount = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            timerFillImage.fillAmount = t / discussionDuration;
            yield return null;
        }

        timerFillImage.fillAmount = 0f;
        gameObject.SetActive(false);

        // NU mag de GameManager naar Vote
        GameManager.Instance.NextPhaseServerRpc();
    }

    private void Clear(Transform t)
    {
        foreach (Transform c in t)
            Destroy(c.gameObject);
    }
}
