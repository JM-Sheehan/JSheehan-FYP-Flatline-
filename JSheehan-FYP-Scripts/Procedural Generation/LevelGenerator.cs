using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LevelGenerator
{

    List<Room> allRooms = new List<Room>();
    private int levelWidth;
    private int levelLength;

    // Level Generator Constructor.
    public LevelGenerator(int levelWidth, int levelLength)
    {
        this.levelWidth = levelWidth;
        this.levelLength = levelLength;
    }

    public List<List<Node>> CalculateLevel(int maxIterations, int roomWidthMin, int roomLengthMin,
        float roomBottomCornerModifier, float roomTopCornerMidifier, int roomOffset, int corridorWidth)
    {
        // Create object for partitioning the level
        Divider div = new Divider(levelWidth, levelLength);

        // Iterates over the space while maintaining the minimum criteria
        allRooms = div.PrepareRooms(maxIterations, roomWidthMin, roomLengthMin);



        // Traverses to lowest point in the tree of nodes then returns list
        List<Node> rooms = Boundaries.TraverseToBottom(div.RootNode);

        Debug.Log("I'm Here");

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);

        // Generate rooms for given nodes and their coordinates
        List<Room> roomList = roomGenerator.SplitIntoRooms(
            rooms, roomBottomCornerModifier, roomTopCornerMidifier, roomOffset);

        // Generates corridors to connect rooms
        CorridorGenerator corridorGenerator = new CorridorGenerator();
        var corridorList = corridorGenerator.CreateCorridor(allRooms, corridorWidth);

        // Keep seperate list for regular rooms and 
        // corridors then return these as a list of lists
        List<Node> corridors = new List<Node>(corridorList.ToList());
        List<List<Node>> lists = new List<List<Node>>();
        lists.Add(rooms);
        lists.Add(corridors);
        return lists;
    }
}