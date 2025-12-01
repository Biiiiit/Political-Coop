using System;

[Serializable]
public class BoardState
{
    public int TurnNumber;
    public int CrisisLevel;
    public Phase Phase;

    public BoardState(int turnNumber, int crisisLevel, Phase phase)
    {
        TurnNumber = turnNumber;
        CrisisLevel = crisisLevel;
        Phase = phase;
    }
}