using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorGenerator
{
    public List<Node> CreateCorridor(List<Room> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<Room> structuresToCheck = new Queue<Room>(
            allNodesCollection.OrderByDescending(node => node.TreeLayerIndex).ToList());
        while (structuresToCheck.Count > 0)
        {
            var node = structuresToCheck.Dequeue();
            if (node.ChildrenNodes.Count == 0)
            {
                continue;
            }
            Corridor corridor = new Corridor(node.ChildrenNodes[0], node.ChildrenNodes[1], corridorWidth);
            corridorList.Add(corridor);
        }
        return corridorList;
    }
}