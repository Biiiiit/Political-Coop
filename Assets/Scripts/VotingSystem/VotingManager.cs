using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class VotingManager : MonoBehaviour
{
    public TMP_Text resultsText;
    public TMP_Text winnerText;

    private List<string> votes = new List<string>();

    private int yesCount = 0;
    private int noCount = 0;

    public string playerName = "Player1";

    public void VoteYes()
    {
        votes.Add(playerName + " voted Yes");
        yesCount++;
        UpdateResults();
    }

    public void VoteNo()
    {
        votes.Add(playerName + " voted No");
        noCount++;
        UpdateResults();
    }

    void UpdateResults()
    {
        resultsText.text = "Results:\n\n";

        foreach (string v in votes)
        {
            resultsText.text += v + "\n\n";
        }
    }

    public void ShowWinner()
    {
        if (yesCount > noCount)
        {
            winnerText.text = "Winner: YES";
            Debug.Log("Winner is YES (" + yesCount + " vs " + noCount + ")");
        }
        else if (noCount > yesCount)
        {
            winnerText.text = "Winner: NO";
            Debug.Log("Winner is NO (" + noCount + " vs " + yesCount + ")");
        }
        else
        {
            winnerText.text = "Winner: TIE";
            Debug.Log("It is a TIE (" + yesCount + " vs " + noCount + ")");
        }
    }
}
