using System.Collections;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [Header("Auto loop settings")]
    [SerializeField] private float loopDelaySeconds = 5f;

    private void Start()
    {
        Debug.Log("[GameBoard] Scene loaded. Looping back in " + loopDelaySeconds + " seconds.");
        StartCoroutine(LoopBackRoutine());
    }

    private IEnumerator LoopBackRoutine()
    {
        yield return new WaitForSeconds(loopDelaySeconds);

        Debug.Log("[GameBoard] 5 seconds passed → requesting next phase.");

        if (GameFlowManager.Instance != null)
        {
            // Call the GameManager's next phase instead
            GameFlowManager.Instance.OnNextPhaseButtonClicked();
        }
        else
        {
            Debug.LogError("[GameBoard] GameFlowManager.Instance is NULL.");
        }
    }
}
