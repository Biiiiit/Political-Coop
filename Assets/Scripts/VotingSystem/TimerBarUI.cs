using UnityEngine;
using UnityEngine.UI;

public class TimerBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Image timerBar;

    [Header("Timer Settings")]
    public float maxTime = 10f;
    private float currentTime;
    private bool timerFinished = false;
    private bool timerRunning = false;

    public Color startColor = Color.red;
    public Color endColor = Color.gray;

    [Header("UI Panels")]
    public GameObject discussionUI;
    public GameObject resultsUI;

    [Header("References")]
    public VotingManager votingManager;

    void Start()
    {
        // Initialize timer and UI state
        currentTime = maxTime;
        timerBar.fillAmount = 1f;
        timerBar.color = startColor;

        discussionUI.SetActive(true);
        resultsUI.SetActive(false);
    }

    void Update()
    {
        if (!timerRunning || timerFinished)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime > 0f)
        {
            float normalizedTime = currentTime / maxTime;
            timerBar.fillAmount = normalizedTime;
            timerBar.color = Color.Lerp(endColor, startColor, normalizedTime);
        }
        else
        {
            EndTimer();
        }
    }

    // Call this to start the timer (e.g., after card animation completes)
    public void StartTimer()
    {
        currentTime = maxTime;
        timerBar.fillAmount = 1f;
        timerBar.color = startColor;
        timerFinished = false;
        timerRunning = true;

        // Ensure discussion UI is active and results UI is hidden
        discussionUI.SetActive(true);
        resultsUI.SetActive(false);
    }

    private void EndTimer()
    {
        timerFinished = true;
        timerRunning = false;

        // Switch UI panels
        discussionUI.SetActive(false);
        resultsUI.SetActive(true);

        // Spawn the card in the results UI
        if (votingManager != null)
        {
            votingManager.ShowWinner();
        }

        Debug.Log("Timer finished: results displayed and card spawned in results UI.");
    }
}
