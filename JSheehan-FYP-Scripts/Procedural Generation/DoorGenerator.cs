using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGenerator : MonoBehaviour
{
    public GameObject door;
    private int doorMultiplier;

    public void SetDoorMultiplier(int mult)
    {
        doorMultiplier = mult;
    }

    public void PlaceDoors(List<Node> corridors, List<Node> rooms)
    {
        foreach(Node corridor in corridors)
        {
            bool check = verticalOpening(corridor, rooms);

            Vector2Int topLeft = corridor.TLCorner;
            Vector2Int topRight = corridor.TRCorner;
            Vector2Int bottomLeft = corridor.BLCorner;
            Vector2Int bottomRight = corridor.BRCorner;
            Vector3 doorPosition = new Vector3(bottomLeft.x + (3 * doorMultiplier / 4), 1, bottomLeft.y);
            GameObject doorObject = Instantiate(door, doorPosition, Quaternion.identity);

            float xScale = door.transform.localScale.x;
            float yScale = door.transform.localScale.y;
            float zScale = door.transform.localScale.z;
            doorObject.transform.localScale = new Vector3(xScale * doorMultiplier, yScale, zScale);

            if (check)
            {
                doorObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                doorPosition = new Vector3(-2+bottomLeft.x + (3 * doorMultiplier / 4), 1, bottomLeft.y +((25*doorMultiplier / 100)));
                doorObject.transform.position = doorPosition;
            }

        }
    }
    bool verticalOpening(Node corridor, List<Node> rooms)
    {
        foreach (Node room in rooms)
        {
            if (corridor.TRCorner.x == room.TLCorner.x)
            {
                return true;
            }
            else if (corridor.TLCorner.x == room.TRCorner.x)
            {
                return true;
            }
        }

        return false;
    }


}
