using UnityEngine;
using TMPro; // <--- Dit is belangrijk voor TextMeshPro
using System.Collections.Generic;

public class VotingManager : MonoBehaviour
{
    public TMP_Text resultsText; // <--- Gebruik TMP_Text i.p.v. Text
    private List<string> votes = new List<string>();

    // Naam van de speler, kan later per kaart aangepast worden
    public string playerName = "Player1";

    // Methode voor Yes stemmen
    public void VoteYes()
    {
        string vote = playerName + " voted Yes";
        votes.Add(vote);
        UpdateResults();
        Debug.Log(vote);
    }

    // Methode voor No stemmen
    public void VoteNo()
    {
        string vote = playerName + " voted No";
        votes.Add(vote);
        UpdateResults();
        Debug.Log(vote);
    }

    // Update de resultaten in de TMP Text
    void UpdateResults()
    {
        resultsText.text = "Results:\n";
        foreach (string v in votes)
        {
            resultsText.text += v + "\n";
        }
    }
}
