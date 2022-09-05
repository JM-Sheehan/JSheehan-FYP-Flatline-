using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Divider
{
    Room rootNode;

    public Room RootNode { get => rootNode; }

    public Divider(int levelWidth, int levelLength)
    {
        // The root of the binary tree is at the bottom left corner of the level
        this.rootNode = new Room(new Vector2Int(0, 0), new Vector2Int(levelWidth, levelLength), null, 0);
    }

    public List<Room> PrepareRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<Room> graph = new Queue<Room>();
        List<Room> listToReturn = new List<Room>();
        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);
        int iterations = 0;

        // The greater the iterations, the more rooms we will end up having and
        // the more consistently they will approach the given minimum dimensions.
        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            Room parentNode = graph.Dequeue();
            if (parentNode.Width >= roomWidthMin * 2 || parentNode.Length >= roomLengthMin * 2)
            {
                BisectSpace(parentNode, listToReturn, roomLengthMin, roomWidthMin, graph);
            }
        }
        return listToReturn;
    }

    private void BisectSpace(Room parentNode, List<Room> listToReturn,
        int roomLengthMin, int roomWidthMin, Queue<Room> graph)
    {
        // Gets line bisecting the given Room Node assuming
        // the bisection meets the necessary requirements.
        Line line = GetLineDividingSpace(
            parentNode.BLCorner,
            parentNode.TRCorner,
            roomWidthMin,
            roomLengthMin);
        Room node1, node2;

        // Split node into top and bottom nodes The resulting nodes are
        // one step farther down the binary tree, therefore their 
        // layer index is one greater than the parent node
        if (line.Orientation == Orientation.Horizontal)
        {
            // Bottom Node.
            node1 = new Room(parentNode.BLCorner,
                new Vector2Int(parentNode.TRCorner.x, line.Coordinates.y),
                parentNode,
                parentNode.TreeLayerIndex + 1);

            // Top Node
            node2 = new Room(new Vector2Int(parentNode.BLCorner.x, line.Coordinates.y),
                parentNode.TRCorner,
                parentNode,
                parentNode.TreeLayerIndex + 1);
        }
        // Split node into left and right nodes
        else
        {
            // left Node
            node1 = new Room(parentNode.BLCorner,
                new Vector2Int(line.Coordinates.x, parentNode.TRCorner.y),
                parentNode,
                parentNode.TreeLayerIndex + 1);
            // Right Node
            node2 = new Room(new Vector2Int(line.Coordinates.x, parentNode.BLCorner.y),
                parentNode.TRCorner,
                parentNode,
                parentNode.TreeLayerIndex + 1);
        }
        AddNewNodeToCollections(listToReturn, graph, node1);
        AddNewNodeToCollections(listToReturn, graph, node2);
    }

    private void AddNewNodeToCollections(List<Room> listToReturn, Queue<Room> graph, Room node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    private Line GetLineDividingSpace(Vector2Int BLCorner, Vector2Int TRCorner, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;
        bool longEnough = (TRCorner.y - BLCorner.y) >= 2 * roomLengthMin;
        bool wideEnough = (TRCorner.x - BLCorner.x) >= 2 * roomWidthMin;

        // Conditions to test which dimensions of the 
        // space are elligible for being bissected.

        if (longEnough && wideEnough)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (wideEnough)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            // Long Enough, but not wide enough
            orientation = Orientation.Horizontal;
        }
        return new Line(orientation, GetCoords(
            orientation,
            BLCorner,
            TRCorner,
            roomWidthMin,
            roomLengthMin));
    }

    private Vector2Int GetCoords(Orientation orientation, Vector2Int BLCorner, Vector2Int TRCorner, int roomWidthMin, int roomLengthMin)
    {
        // Defines the point at which a dividing line is drawn

        Vector2Int coordinates = Vector2Int.zero;
        if (orientation == Orientation.Horizontal)
        {
            // X at the left side of space y randomized within the
            // spaces bounds.
            coordinates = new Vector2Int(
                0,
                Random.Range(
                (BLCorner.y + roomLengthMin),
                (TRCorner.y - roomLengthMin)));
        }
        else
        {
            // Y at the bottom side of space x randomized within the
            // spaces bounds.
            coordinates = new Vector2Int(
                Random.Range(
                (BLCorner.x + roomWidthMin),
                (TRCorner.x - roomWidthMin))
                , 0);
        }
        return coordinates;
    }
}