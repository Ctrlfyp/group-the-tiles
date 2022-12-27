using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class GroupTileUtility
{
    public static GameTileGroup GetExistingGroup(List<GameTileGroup> groups, Vector2 location)
    {
        return groups.FirstOrDefault(g => g.tiles.ContainsKey(location));
    }

    public static GameTileGroup CreateGroup(List<GameTileGroup> groups)
    {
        GameTileGroup newGroup = new GameTileGroup();
        groups.Add(newGroup);

        for (int i = 0; i < groups.Count; i++)
        {
            groups[i].SetSign(i.ToString());
            groups[i].SetBorders();
        }

        return newGroup;
    }

    public static void RemoveGroup(List<GameTileGroup> groups, GameTileGroup group)
    {
        group.ClearGroup();
        groups.Remove(group);
    }
}
