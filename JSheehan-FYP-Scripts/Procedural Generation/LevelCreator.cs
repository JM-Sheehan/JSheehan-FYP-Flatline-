using ProBuilder2.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using static UnityEngine.Rendering.DebugUI.Table;

public class LevelCreator : MonoBehaviour
{

    public GameObject player;
    public GameObject patrolPoint;
    public GameObject escapeObject;

    private int escapeRoomIndex;
    private int spawnRoomIndex;

    public int patrolPointVerticalOffset = 5;
    public int patrolPointHorizontalOffset = 5;

    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public float ceilingHeight;
    public Material floorMaterial;
    public Material ceilingMaterial;
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [Range(0, 2)]
    public int roomOffset;

    public GameObject wallVertical, wallHorizontal;

    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;

    List<Vector2Int> spawnPositions;

    List<GameObject> meshes;

    List<Node> corridors;
    List<Node> rooms;

    public List<GameObject> walls;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<DoorGenerator>().SetDoorMultiplier(corridorWidth);
        walls = new List<GameObject>();
        corridors = new List<Node>();
        rooms = new List<Node>();
        spawnPositions = new List<Vector2Int>();
        meshes = new List<GameObject> ();
        CreateDungeon();
        GetComponent<DoorGenerator>().PlaceDoors(corridors,rooms);
        foreach (Node room in rooms) PreparePatrolPoints(room);
        CalculateEscapeAndSpawnPoint();
        PrepareHidingSpots();
        PrepareSpawnAndEscape();
        GameObject[] hidingCams = GameObject.FindGameObjectsWithTag("HidingCam");
        foreach(GameObject cam in hidingCams)
        {
            cam.SetActive(false);
        }
        PlaceModels();

    }

    private void CalculateEscapeAndSpawnPoint()
    {
        escapeRoomIndex = UnityEngine.Random.Range(0, rooms.Count);
        Node escapeRoom = rooms[escapeRoomIndex];

        int farthestRoomIndex = escapeRoomIndex;
        float maxDistance = 0;
        Vector2Int escapeMiddle = MiddleOfNode(escapeRoom);
        for(int i=0; i< rooms.Count; i++)
        {
            float currentDistance = Vector2.Distance(escapeMiddle, MiddleOfNode(rooms[i]));
            if (currentDistance > maxDistance ){
                maxDistance = currentDistance;
                farthestRoomIndex = i;
            }
        }
        spawnRoomIndex = farthestRoomIndex;
    }

    private void PrepareSpawnAndEscape()
    {
        Vector2Int twoDCoords;
        twoDCoords = MiddleOfNode(rooms[escapeRoomIndex]);
        Vector3 position = new Vector3(twoDCoords.x, 3, twoDCoords.y);
        GameObject escape = Instantiate(escapeObject, position, Quaternion.identity);

    }

    private Vector2Int MiddleOfNode(Node node)
    {
        int startX = node.BLCorner.x;
        int startY = node.BLCorner.y;
        int endX = node.BRCorner.x;
        int endY = node.TLCorner.y;

        int xCord = startX + ((endX - startX) / 2);
        int yCord = startY + ((endY - startY) / 2);

        return new Vector2Int(xCord, yCord);
    }

    private void Update()
    {
        player.GetComponent<FirstPersonController>().teleporting = false;
    }

    public void CreateDungeon()
    {
        DestroyAllChildren();
        LevelGenerator generator = new LevelGenerator(dungeonWidth, dungeonLength);
        var generatedList = generator.CalculateLevel(maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerMidifier,
            roomOffset,
                corridorWidth);
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        List<Node> listOfRooms = generatedList[0];
        foreach(Node room in listOfRooms)
        {
            rooms.Add(room);
        }
        List<Node> listTwo = generatedList[1];
        corridors = listTwo;
        listOfRooms.AddRange(listTwo);

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BLCorner, listOfRooms[i].TRCorner);

            Vector2Int bottomlc = listOfRooms[i].BLCorner;
            Vector2Int toprc = listOfRooms[i].TRCorner;

            int xDiff = toprc.x - bottomlc.x;
            int yDIff = toprc.y - bottomlc.y;

            int xCoord = bottomlc.x + (xDiff / 2);
            int yCoord = bottomlc.y + (yDIff / 2);
            Vector2Int coordinates = new Vector2Int(xCoord, yCoord);
            spawnPositions.Add(coordinates);
        }
        CreateWalls(wallParent);

        foreach (GameObject obj in meshes)
        {
            obj.GetComponent<NavMeshSurface>().BuildNavMesh();
        }

    }



    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            wallHorizontal = GetComponent<WallGenerator>().chooseHorizontal();
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            wallVertical = GetComponent<WallGenerator>().chooseVertical();
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }


    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
        walls.Add(wall);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3 bottomLeftVC = new Vector3(bottomLeftCorner.x, ceilingHeight, bottomLeftCorner.y);
        Vector3 bottomRightVC = new Vector3(topRightCorner.x, ceilingHeight, bottomLeftCorner.y);
        Vector3 topLeftVC = new Vector3(bottomLeftCorner.x, ceilingHeight, topRightCorner.y);
        Vector3 topRightVC = new Vector3(topRightCorner.x, ceilingHeight, topRightCorner.y);


        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector3[] verticesC = new Vector3[]
        {
            topLeftVC,
            topRightVC,
            bottomLeftVC,
            bottomRightVC
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        Vector2[] uvsC = new Vector2[verticesC.Length];
        for (int i = 0; i < uvsC.Length; i++)
        {
            uvsC[i] = new Vector2(verticesC[i].x, verticesC[i].z);
        }

        int[] upwardsTriangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };

        int[] downwardsTriangles = new int[]
        {
            2,
            1,
            0,
            3,
            1,
            2
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = upwardsTriangles;

        Mesh ceilingMeshDown = new Mesh();
        ceilingMeshDown.vertices = verticesC;
        ceilingMeshDown.uv = uvsC;
        ceilingMeshDown.triangles = downwardsTriangles;

        Mesh ceilingMeshUp = new Mesh();
        ceilingMeshUp.vertices = verticesC;
        ceilingMeshUp.uv = uvsC;
        ceilingMeshUp.triangles = upwardsTriangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(NavMeshSurface));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = floorMaterial;
        dungeonFloor.GetComponent<MeshCollider>().sharedMesh = null;
        dungeonFloor.GetComponent<MeshCollider>().sharedMesh = dungeonFloor.GetComponent<MeshFilter>().sharedMesh;

        dungeonFloor.isStatic = true;
        meshes.Add(dungeonFloor);
        dungeonFloor.transform.parent = transform;

        GameObject dungeonCeilingDown = new GameObject("Ceiling" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonCeilingDown.transform.position = Vector3.zero;
        dungeonCeilingDown.transform.localScale = Vector3.one;
        dungeonCeilingDown.GetComponent<MeshFilter>().mesh = ceilingMeshDown;
        dungeonCeilingDown.GetComponent<MeshRenderer>().material = ceilingMaterial;
        dungeonCeilingDown.transform.parent = transform;


        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point)){
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while(transform.childCount != 0)
        {
            foreach(Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }

    private void PlaceModels()
    {
        Node spawnNode = rooms[spawnRoomIndex];

        player.GetComponent<FirstPersonController>().teleporting = true;

        Vector2Int position = MiddleOfNode(spawnNode);

        float playerX = (float)position.x;
        float playerY = 0f;
        float playerZ = (float)position.y;

        Vector3 playerPositionVector = new Vector3(playerX, playerY, playerZ);

        player.transform.position = playerPositionVector;
        player.GetComponent<FirstPersonController>().transform.position = playerPositionVector;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {

            int enemySpawn = RandomSpawn();
            Vector2Int enemyPosition = spawnPositions[enemySpawn];

            float enemyX = (float)enemyPosition.x;
            float enemyY = enemy.transform.position.y;
            float enemyZ = (float)enemyPosition.y;
            enemy.transform.position = new Vector3(enemyX, enemyY, enemyZ);
            enemy.SetActive(true);
        }

    }

    private int RandomSpawn()
    {
        int max = spawnPositions.Count;
        int spawn = UnityEngine.Random.Range(0, max);
        return spawn;

    }

    public List<Node> GetCorridors()
    {
        return corridors;
    }

    public void PreparePatrolPoints(Node room)
    {
        Vector2Int bottomLeft = room.BLCorner;
        Vector2Int bottomRight = room.BRCorner;
        Vector2Int topLeft = room.TLCorner;
        Vector2Int topRight = room.TRCorner;

        if (
            bottomRight.x - bottomLeft.x <= patrolPointHorizontalOffset &&
            !(topLeft.y - bottomLeft.y <= patrolPointVerticalOffset)
          )
        {
            int xCord = bottomLeft.x + (bottomRight.x - bottomLeft.x) / 2;

            int yCord = bottomLeft.y + patrolPointVerticalOffset;
            while (yCord < topLeft.y)
            {
                Vector3 position = new Vector3(xCord, 1, yCord);
                GameObject pp = Instantiate(patrolPoint, position, Quaternion.identity);
                yCord += patrolPointVerticalOffset;
            }
        }
        else if (
            topLeft.y - bottomLeft.y <= patrolPointVerticalOffset &&
            !(bottomRight.x - bottomLeft.x <= patrolPointHorizontalOffset)
          )
        {
            int xCord = bottomLeft.x + patrolPointHorizontalOffset;

            int yCord = bottomLeft.y + (topLeft.y - bottomLeft.y) / 2;
            while (xCord < bottomRight.x)
            {
                Vector3 position = new Vector3(xCord, 1, yCord);
                GameObject pp = Instantiate(patrolPoint, position, Quaternion.identity);
                xCord += patrolPointHorizontalOffset;
            }
        }
        else if (
            topLeft.y - bottomLeft.y <= patrolPointVerticalOffset &&
            (bottomRight.x - bottomLeft.x <= patrolPointHorizontalOffset)
            )
        {
            int xCord = bottomLeft.x + (corridorWidth / 2);
            int yCord = bottomLeft.y + (corridorWidth/ 2);
            Vector3 position = new Vector3(xCord, 1, yCord);
            GameObject pp = Instantiate(patrolPoint, position, Quaternion.identity);

        }
        else
        {
            int xCord = bottomLeft.x + patrolPointHorizontalOffset;


            while (xCord < bottomRight.x)
            {
                int yCord = bottomLeft.y + patrolPointVerticalOffset;
                while (yCord < topLeft.y)
                {
                    Vector3 position = new Vector3(xCord, 1, yCord);
                    GameObject pp = Instantiate(patrolPoint, position, Quaternion.identity);

                    yCord += patrolPointVerticalOffset;
                }
                xCord += patrolPointHorizontalOffset;
            }
        }

    }

    void PrepareHidingSpots()
    {
        List<Node> hidingRooms = new List<Node>();
        for(int i= 0;i< rooms.Count; i ++)
        {
            if(i != escapeRoomIndex && i != spawnRoomIndex)
            {
                hidingRooms.Add(rooms[i]);
            }
        }

        GetComponent<HidingSpotGenerator>().placeSpotsInRooms(hidingRooms);
    }


}
