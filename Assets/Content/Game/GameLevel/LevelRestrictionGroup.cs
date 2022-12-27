using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelRestrictionGroup : LevelRestriction
{
    [JsonIgnore]
    private List<LevelRestriction> _restrictions;
    public List<LevelRestriction> restrictions 
    {
        get
        {
            return _restrictions;
        }
        set
        {
            foreach (LevelRestriction levelRestriction in value)
            {
                levelRestriction.parent = this;
                if (levelRestriction is LevelRestrictionGroup)
                {
                    (levelRestriction as LevelRestrictionGroup).ReparentChildren();
                }
            }
            _restrictions = value;
        }
    }
    public BOOLEAN_OPERATOR groupOperator;

    public LevelRestrictionGroup()
    {
        groupOperator = BOOLEAN_OPERATOR.AND;
        restrictions = new List<LevelRestriction>();
    }

    public LevelRestrictionGroup(List<LevelRestriction> restrictions, BOOLEAN_OPERATOR groupOperator)
    {
        this.restrictions = restrictions;
        this.groupOperator = groupOperator;
    }

    public override bool CheckSubmittion(List<GameTileGroup> groups)
    {
        bool preinvertedFinal = true;

        if (restrictions.Count >= 0)
        {
            switch (groupOperator)
            {
                case BOOLEAN_OPERATOR.AND:
                    foreach (LevelRestriction levelRestriction in restrictions)
                    {
                        if (!levelRestriction.CheckSubmittion(groups))
                        {
                            preinvertedFinal = false;
                            break;
                        }
                    }
                    break;
                case BOOLEAN_OPERATOR.OR:
                    foreach (LevelRestriction levelRestriction in restrictions)
                    {
                        if (levelRestriction.CheckSubmittion(groups))
                        {
                            preinvertedFinal = true;
                            break;
                        }
                    }
                    break;
                case BOOLEAN_OPERATOR.XOR:
                    foreach (LevelRestriction levelRestriction in restrictions)
                    {
                        if (levelRestriction.CheckSubmittion(groups))
                        {
                            if (preinvertedFinal)
                            {
                                preinvertedFinal = false;
                                break;
                            }
                            preinvertedFinal = true;
                        }
                    }
                    break;
            }
        }

        if (negated)
        {
            return !preinvertedFinal;
        }

        return preinvertedFinal;
    }

    public void ReparentChildren()
    {
        foreach (LevelRestriction levelRestriction in _restrictions)
        {
            levelRestriction.parent = this;
            if (levelRestriction is LevelRestrictionGroup)
            {
                (levelRestriction as LevelRestrictionGroup).ReparentChildren();
            }
        }
    }

    public void AddLevelRestriction(LevelRestriction levelRestriction)
    {
        restrictions.Add(levelRestriction);
        levelRestriction.parent = this;
    }
    
    public void RemoveLevelRestriction(LevelRestriction levelRestriction)
    {
        restrictions.Remove(levelRestriction);
    }

    public void ReplaceLevelRestriction(LevelRestrictionClause oldClause, LevelRestrictionClause newClause)
    {
        int index = restrictions.IndexOf(oldClause);
        RemoveLevelRestriction(oldClause);
        restrictions.Insert(index, newClause);
        newClause.parent = this;
    }
}
