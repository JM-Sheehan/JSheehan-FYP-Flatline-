using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Corridor : Node
{
    // Rooms to be connected by corridor
    private Node roomOne;
    private Node roomTwo;

    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public Corridor(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.roomOne = node1;
        this.roomTwo = node2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    // Creates corridor with relative position depenednt 
    // on the positions of the two rooms it is connecting
    private void GenerateCorridor()
    {
        var relativePositionOfroomTwo = compareRoomsPositions();
        switch (relativePositionOfroomTwo)
        {
            case RelativePosition.Up:
                CheckVerticalRelation(this.roomOne, this.roomTwo);
                break;
            case RelativePosition.Down:
                CheckVerticalRelation(this.roomTwo, this.roomOne);
                break;
            case RelativePosition.Right:
                CheckHorizontalRelation(this.roomOne, this.roomTwo);
                break;
            case RelativePosition.Left:
                CheckHorizontalRelation(this.roomTwo, this.roomOne);
                break;
            default:
                break;
        }
    }

    // Check if Rooms Are Connected Horizontally
    private void CheckHorizontalRelation(Node roomOne, Node roomTwo)
    {
        // Traverse to the bottom of tree starting at both given nodes and 
        // build list of their children.
        Node leftRoom = null;
        List<Node> leftRoomChildren = Boundaries.TraverseToBottom(roomOne);
        Node rightRoom = null;
        List<Node> rightRoomChildren = Boundaries.TraverseToBottom(roomTwo);

        // Orders list according to x coordinates
        var sortedleftRoom = leftRoomChildren.OrderByDescending(child => child.TRCorner.x).ToList();
        if (sortedleftRoom.Count == 1)
        {
            leftRoom = sortedleftRoom[0];
        }
        else
        {
            int maxX = sortedleftRoom[0].TRCorner.x;
            sortedleftRoom = sortedleftRoom.Where(children => Math.Abs(maxX - children.TRCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedleftRoom.Count);
            leftRoom = sortedleftRoom[index];
        }

        var possibleRightNeighbours = rightRoomChildren.Where(
            child => GetHorizontalNeighbourYCoordinate(
                leftRoom.TRCorner,
                leftRoom.BRCorner,
                child.TLCorner,
                child.BLCorner
                ) != -1
            ).OrderBy(child => child.BRCorner.x).ToList();

        if (possibleRightNeighbours.Count <= 0)
        {
            rightRoom = roomTwo;
        }
        else
        {
            rightRoom = possibleRightNeighbours[0];
        }
        int y = GetHorizontalNeighbourYCoordinate(leftRoom.TLCorner, leftRoom.BRCorner,
            rightRoom.TLCorner,
            rightRoom.BLCorner);
        while (y == -1 && sortedleftRoom.Count > 1)
        {
            sortedleftRoom = sortedleftRoom.Where(
                child => child.TLCorner.y != leftRoom.TLCorner.y).ToList();
            leftRoom = sortedleftRoom[0];
            y = GetHorizontalNeighbourYCoordinate(leftRoom.TLCorner, leftRoom.BRCorner,
            rightRoom.TLCorner,
            rightRoom.BLCorner);
        }
        BLCorner = new Vector2Int(leftRoom.BRCorner.x, y);
        TRCorner = new Vector2Int(rightRoom.TLCorner.x, y + this.corridorWidth);
    }

    private int GetHorizontalNeighbourYCoordinate(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return Boundaries.FindMidPoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return Boundaries.FindMidPoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return Boundaries.FindMidPoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall)
                ).y;
        }
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return Boundaries.FindMidPoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        return -1;
    }

    private void CheckVerticalRelation(Node roomOne, Node roomTwo)
    {
        Node bottomStructure = null;
        List<Node> structureBottmChildren = Boundaries.TraverseToBottom(roomOne);
        Node topStructure = null;
        List<Node> structureAboveChildren = Boundaries.TraverseToBottom(roomTwo);

        var sortedBottomStructure = structureBottmChildren.OrderByDescending(child => child.TRCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottmChildren[0];
        }
        else
        {
            int maxY = sortedBottomStructure[0].TLCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TLCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var verticalNeighbours = structureAboveChildren.Where(
            child => GetVerticalNeighbourXCoordinate(
                bottomStructure.TLCorner,
                bottomStructure.TRCorner,
                child.BLCorner,
                child.BRCorner)
            != -1).OrderBy(child => child.BRCorner.y).ToList();
        if (verticalNeighbours.Count == 0)
        {
            topStructure = roomTwo;
        }
        else
        {
            topStructure = verticalNeighbours[0];
        }
        int x = GetVerticalNeighbourXCoordinate(
                bottomStructure.TLCorner,
                bottomStructure.TRCorner,
                topStructure.BLCorner,
                topStructure.BRCorner);
        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(child => child.TLCorner.x != topStructure.TLCorner.x).ToList();
            bottomStructure = sortedBottomStructure[0];
            x = GetVerticalNeighbourXCoordinate(
                bottomStructure.TLCorner,
                bottomStructure.TRCorner,
                topStructure.BLCorner,
                topStructure.BRCorner);
        }
        BLCorner = new Vector2Int(x, bottomStructure.TLCorner.y);
        TRCorner = new Vector2Int(x + this.corridorWidth, topStructure.BLCorner.y);
    }

    // Gets A Valid X Coordinate for constructing corridor to connect two 
    // Vertical Neighbours

    private int GetVerticalNeighbourXCoordinate(Vector2Int bottomNodeLeft,
        Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return Boundaries.FindMidPoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return Boundaries.FindMidPoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return Boundaries.FindMidPoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return Boundaries.FindMidPoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        return -1;
    }

    // Uses Calculated Angle and Assigns this a corresponding
    // Value of the Enumerator RelativePosition.
    private RelativePosition compareRoomsPositions()
    {
        Vector2 middlePointroomOneTemp = ((Vector2)roomOne.TRCorner + roomOne.BLCorner) / 2;
        Vector2 middlePointroomTwoTemp = ((Vector2)roomTwo.TRCorner + roomTwo.BLCorner) / 2;
        float angle = CalculateAngle(middlePointroomOneTemp, middlePointroomTwoTemp);
        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if (angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    // Compares the middle point of two rooms to calculate
    // The angle of a theoretical line between the rooms.
    private float CalculateAngle(Vector2 middlePointroomOneTemp, Vector2 middlePointroomTwoTemp)
    {
        return Mathf.Atan2(middlePointroomTwoTemp.y - middlePointroomOneTemp.y,
            middlePointroomTwoTemp.x - middlePointroomOneTemp.x) * Mathf.Rad2Deg;
    }
}