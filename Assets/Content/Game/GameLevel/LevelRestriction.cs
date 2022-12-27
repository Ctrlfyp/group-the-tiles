using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class LevelRestriction
{
    public enum BOOLEAN_OPERATOR
    {
        AND, OR, XOR
    }
    [NonSerialized]
    public LevelRestrictionGroup parent;
    public bool negated;
    public abstract bool CheckSubmittion(List<GameTileGroup> groups);
}
