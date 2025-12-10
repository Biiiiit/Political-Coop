using System;

[Serializable]
public class SectorState
{
    public Role Role;
    public int ResourceLevel;

    public SectorState(Role role)
    {
        Role = role;
        ResourceLevel = 0;
    }
}
