using UnityEngine;

public class GameScreen : MonoBehaviour
{
    public static GameScreen Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private GameScreenSyncHelper syncHelper;

    [Header("UI")]
    [SerializeField] private GameObject deckCanvasGO; // Draw/cards UI
    [SerializeField] private GameObject votingUI;     // Voting UI (inside this scene)

    [Header("Auto Flow")]
    [SerializeField] private bool autoStartDrawOnSceneLoad = true;

    [Tooltip("If true, voting ends automatically after delay.")]
    [SerializeField] private bool autoFinishVoting = true;

    [SerializeField] private float autoFinishVotingDelay = 5f;

    private bool drawStarted = false;
    private bool cardPlayed = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // If flow manager exists, this scene is likely loaded by it.
        // But even if you press Play directly in this scene, it will work.
        if (autoStartDrawOnSceneLoad)
            StartDrawPhase();
    }

    public void StartDrawPhase()
    {
        if (drawStarted) return;
        drawStarted = true;

        cardPlayed = false;

        if (deckCanvasGO != null) deckCanvasGO.SetActive(true);
        if (votingUI != null) votingUI.SetActive(false);

        if (deckManager == null)
        {
            Debug.LogError("[GameScreen] DeckManager is not assigned.");
            return;
        }

        Debug.Log("[GameScreen] Starting draw phase.");
        deckManager.DrawCustomSequence();
    }

    /// <summary>
    /// Called by HandManager after the player plays a card.
    /// </summary>
    public void OnCardPlayed(string cardId = "", string cardTitle = "")
    {
        if (cardPlayed) return;
        cardPlayed = true;

        Debug.Log("[GameScreen] Card played -> Voting phase started.");

        // Sync card play to GameBoard
        if (syncHelper != null && !string.IsNullOrEmpty(cardId))
        {
            syncHelper.OnCardPlayed(cardId, cardTitle);
        }

        if (deckCanvasGO != null) deckCanvasGO.SetActive(false);
        if (votingUI != null) votingUI.SetActive(true);

        if (autoFinishVoting)
        {
            CancelInvoke(nameof(FinishVoting));
            Invoke(nameof(FinishVoting), autoFinishVotingDelay);
        }
    }

    /// <summary>
    /// Call this from VotingManager when voting ends OR let the auto timer call it.
    /// </summary>
    public void FinishVoting()
    {
        Debug.Log("[GameScreen] FinishVoting called -> showing results on shared screen");
        
        // No scene transition needed - shared screen will show board in same scene
        // Just notify that voting is complete
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnNextPhaseButtonClicked();
        }
        else
        {
            Debug.Log("[GameScreen] Voting complete. Results should display on shared screen.");
        }
    }
}
