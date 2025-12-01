[System.Serializable]
public class BoardState
{
    public int TurnNumber;
    public int CrisisLevel;

    public BoardState(int turnNumber, int crisisLevel)
    {
        TurnNumber = turnNumber;
        CrisisLevel = crisisLevel;
    }
}