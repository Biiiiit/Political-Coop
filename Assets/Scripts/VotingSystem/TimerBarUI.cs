using UnityEngine;
using UnityEngine.UI;

public class TimerBarUI : MonoBehaviour
{
    public Image timerBar;

    public float maxTime = 10f;
    private float currentTime;
    private bool timerFinished = false;

    public Color startColor = Color.red;
    public Color endColor = Color.gray;

    public GameObject discussionUI;
    public GameObject resultsUI;

    public VotingManager votingManager;

    void Start()
    {
        currentTime = maxTime;
        timerBar.fillAmount = 1f;
        timerBar.color = startColor;

        discussionUI.SetActive(true);
        resultsUI.SetActive(false);
    }

    void Update()
    {
        if (timerFinished)
            return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            float normalizedTime = currentTime / maxTime;
            timerBar.fillAmount = normalizedTime;
            timerBar.color = Color.Lerp(endColor, startColor, normalizedTime);
        }
        else
        {
            EndTimer();
        }
    }

    void EndTimer()
    {
        timerFinished = true;

        discussionUI.SetActive(false);
        resultsUI.SetActive(true);

        votingManager.ShowWinner();
    }
}
