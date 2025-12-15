using UnityEngine;

public enum CardCategory
{
    Policy,
    Project,
    ShortTerm
}

[System.Serializable]
public class CardData
{
    public string id;
    public string title;
    public CardCategory category;

    public string GetTypeLabel()
    {
        switch (category)
        {
            case CardCategory.Policy:
                return "Policy";
            case CardCategory.Project:
                return "Project";
            case CardCategory.ShortTerm:
                return "short-term";
            default:
                return string.Empty;
        }
    }
}
