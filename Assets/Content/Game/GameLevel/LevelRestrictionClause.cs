using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public abstract class LevelRestrictionClause : LevelRestriction
{
    [Serializable]
    public class GroupTileCountClause : LevelRestrictionClause
    {
        public int minSize = -1;
        public int requiredSize = -1;
        public int maxSize = -1;

        public GroupTileCountClause()
        {

        }

        public GroupTileCountClause(int minSize, int requiredSize, int maxSize)
        {
            this.minSize = minSize;
            this.requiredSize = requiredSize;
            this.maxSize = maxSize;
        }

        public override bool CheckSubmittion(List<GameTileGroup> groups)
        {
            bool pre = (requiredSize == groups[0].tiles.Count || requiredSize == -1)
                && (minSize <= groups[0].tiles.Count || minSize == -1)
                && (maxSize >= groups[0].tiles.Count || maxSize == -1);

            if (negated)
            {
                return !pre;
            }

            return pre;
        }
    }

    [Serializable]
    public class GroupCountClause : LevelRestrictionClause
    {
        public int minSize = -1;
        public int requiredSize = -1;
        public int maxSize = -1;

        public GroupCountClause()
        {

        }

        public GroupCountClause(int minSize, int requiredSize, int maxSize)
        {
            this.minSize = minSize;
            this.requiredSize = requiredSize;
            this.maxSize = maxSize;
        }

        public override bool CheckSubmittion(List<GameTileGroup> groups)
        {
            bool pre = (requiredSize == groups.Count || requiredSize == -1)
                && (minSize <= groups.Count || minSize == -1)
                && (maxSize >= groups.Count || maxSize == -1);

            if (negated)
            {
                return !pre;
            }

            return pre;
        }
    }

    [Serializable]
    public class GroupSpecificClause : LevelRestrictionClause
    {
        public Dictionary<int, int> referenceTileValues;

        public GroupSpecificClause()
        {
            referenceTileValues = new Dictionary<int, int>();
        }

        public GroupSpecificClause(Dictionary<int, int> referenceTileValues)
        {
            this.referenceTileValues = referenceTileValues;
        }

        public override bool CheckSubmittion(List<GameTileGroup> groups)
        {
            bool pre = referenceTileValues.All(e => groups[0].tiles.Count(t =>
                t.Value.tileType == GameTile.TILE_TYPE.RAINBOW ||
                t.Value.gameTile.colourIndex == e.Key
            ) == e.Value);

            if (negated)
            {
                return !pre;
            }

            return pre;
        }
    }

    [Serializable]
    public class GroupMinimumClause : LevelRestrictionClause
    {
        public Dictionary<int, int> referenceTileValues;

        public GroupMinimumClause()
        {
            referenceTileValues = new Dictionary<int, int>();
        }

        public GroupMinimumClause(Dictionary<int, int> referenceTileValues)
        {
            this.referenceTileValues = referenceTileValues;
        }

        public override bool CheckSubmittion(List<GameTileGroup> groups)
        {
            bool pre = referenceTileValues.All(e => groups[0].tiles.Count(t =>
                t.Value.tileType == GameTile.TILE_TYPE.RAINBOW ||
                t.Value.gameTile.colourIndex == e.Key
            ) >= e.Value);

            if (negated)
            {
                return !pre;
            }

            return pre;
        }
    }
}
