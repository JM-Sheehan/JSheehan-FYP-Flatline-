using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpotGenerator : MonoBehaviour
{
    public List<GameObject> hidingPrefabs;
    public List<GameObject> nonHidingPrefabs;
    public GameObject tempObject;
    public Vector2Int minClearance;
    public int spawnChance = 4;
    public int nonHidableSpawnChance = 3;
    // Start is called before the first frame update

    public void placeSpotsInRooms(List<Node> rooms)
    {
        foreach(Node room in rooms)
        {
            int width = (room.BRCorner.x) - (room.BLCorner.x);
            int length = (room.TLCorner.y) - (room.BLCorner.y);
            if(width > minClearance.x && length > minClearance.y)
            {
                List<List<Vector2Int>> splitUpRoom = AllocateSpots(room, width, length, room.BLCorner.x, room.BRCorner.y);
                randomlyPlaceHidingSpots(splitUpRoom);
            }
        }
    }

    private void randomlyPlaceHidingSpots(List<List<Vector2Int>> splitUpRooms)
    {

        foreach(List<Vector2Int> points in splitUpRooms)
        {
            int result = UnityEngine.Random.Range(1, spawnChance + 1);
            Vector2Int bL = points[0];
            Vector2Int tR = points[1];
            int xCord = bL.x + ((tR.x - bL.x) / 2);
            int zCord = bL.y + ((tR.y - bL.y) / 2);

            Vector3 position = new Vector3(xCord, 0, zCord);
            if (result == 1)
            { 
                int randomPrefab = UnityEngine.Random.Range(0, hidingPrefabs.Count);

                if (randomPrefab == 2) position.y = 3.1f;
                Instantiate(hidingPrefabs[randomPrefab], position, Quaternion.identity);
            }
            else
            {
                int res = UnityEngine.Random.Range(1, nonHidableSpawnChance + 1);
                if(res == 1)
                {
                    int randomPrefab = UnityEngine.Random.Range(0, nonHidingPrefabs.Count);
                    Instantiate(nonHidingPrefabs[randomPrefab], position, Quaternion.identity);

                }
            }
        }

    }

    List<List<Vector2Int>> AllocateSpots(Node room, int width, int length, int startingX, int startingY)
    {
        List<List<Vector2Int>> roomSplits = new List<List<Vector2Int>>();
        int remainingWidth = width;
        int remainingLength = length;

        while (remainingLength > minClearance.y)
        {
            while(remainingWidth > minClearance.x)
            {
                Vector2Int topRight = new Vector2Int(startingX + remainingWidth, startingY + remainingLength);
                Vector2Int bottomLeft = topRight - minClearance;

                List<Vector2Int> currentRegion = new List<Vector2Int>();;
                currentRegion.Add(bottomLeft);
                currentRegion.Add(topRight);
                roomSplits.Add(currentRegion);
                remainingWidth -= minClearance.x;
            }
            remainingWidth = width;
            remainingLength -= minClearance.y;
        }
        return roomSplits;

    }
}
