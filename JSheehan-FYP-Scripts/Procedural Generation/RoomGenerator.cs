using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    private int maxIterations;
    private int roomLengthMin;
    private int roomWidthMin;

    public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
    {
        this.maxIterations = maxIterations;
        this.roomLengthMin = roomLengthMin;
        this.roomWidthMin = roomWidthMin;
    }

    // Iterates over the list of Nodes corresponding to room
    // spaces, then using the helper class 
    public List<Room> SplitIntoRooms(List<Node> roomSpaces, float roomBottomCornerModifier,
        float roomTopCornerMidifier, int roomOffset)
    {
        List<Room> result = new List<Room>();
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = Boundaries.FindBLCorner(
                space.BLCorner, space.TRCorner, roomBottomCornerModifier, roomOffset);

            Vector2Int newTopRightPoint = Boundaries.FindTRCorner(
                space.BLCorner, space.TRCorner, roomTopCornerMidifier, roomOffset);
            space.BLCorner = newBottomLeftPoint;
            space.TRCorner = newTopRightPoint;
            space.BRCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TLCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            result.Add((Room)space);

        }
        return result;
    }
}