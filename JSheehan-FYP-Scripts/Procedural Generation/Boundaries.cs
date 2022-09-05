using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Boundaries
{
    public static List<Node> TraverseToBottom(Node parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> result = new List<Node>();
        if (parentNode.ChildrenNodes.Count == 0)
        {
            return new List<Node>() { parentNode };
        }
        foreach (var child in parentNode.ChildrenNodes)
        {
            nodesToCheck.Enqueue(child);
        }
        while (nodesToCheck.Count > 0)
        {
            var currentNode = nodesToCheck.Dequeue();

            // Has Reached bottom of this particular branch add the parents of nodes
            // to result while condition remains true.
            if (currentNode.ChildrenNodes.Count == 0)
            {
                result.Add(currentNode);
            }

            // Adds all children to end queue.
            else
            {
                foreach (var child in currentNode.ChildrenNodes)
                {
                    nodesToCheck.Enqueue(child);
                }
            }
        }
        return result;
    }

    public static Vector2Int FindBLCorner(
        Vector2Int areaLP, Vector2Int areaRP, float pointModifier, int offset)
    {
        int minX = areaLP.x + offset;
        int maxX = areaRP.x - offset;
        int minY = areaLP.y + offset;
        int maxY = areaRP.y - offset;
        return new Vector2Int(
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (minY - minY) * pointModifier)));
    }

    public static Vector2Int FindTRCorner(
        Vector2Int areaLP, Vector2Int areaRP, float pointModifier, int offset)
    {
        int minX = areaLP.x + offset;
        int maxX = areaRP.x - offset;
        int minY = areaLP.y + offset;
        int maxY = areaRP.y - offset;
        return new Vector2Int(
            Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY)
            );
    }


    // Finds the Mid point of Node using vector based addition and division.
    public static Vector2Int FindMidPoint(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;
        return new Vector2Int((int)tempVector.x, (int)tempVector.y);
    }
}

// Used to discover orientation relative to Neighbours
public enum RelativePosition
{
    Up,
    Down,
    Right,
    Left
}