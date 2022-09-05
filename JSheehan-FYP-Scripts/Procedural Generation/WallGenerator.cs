using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{

    public List<int> horizontalWeight = new List<int>();
    public List<GameObject> horizontalWalls = new List<GameObject>();

    public List<int> verticalWeight = new List<int>();
    public List<GameObject> verticalWalls = new List<GameObject>();

    public int horizontalTotal;
    public int verticalTotal;

    public List<int> verticalRanges = new List<int>();
    public List<int> horizontalRanges = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        int hTotal = 0;
        int vTotal = 0;

        foreach(int w in horizontalWeight)
        {
            hTotal += w;
            horizontalRanges.Add(hTotal);
        }

        foreach(int w in verticalWeight)
        {
            vTotal += w;
            verticalRanges.Add(vTotal);
        }
        horizontalTotal = hTotal;
        verticalTotal = vTotal;
    }

    public GameObject chooseVertical()
    {
        int rand = Random.Range(0, verticalTotal);
        GameObject result;

        for(int i = 0; i < verticalRanges.Count; i++)
        {
            if (rand < verticalRanges[i])
            {
                result = verticalWalls[i];
                return result;
            }

        }
        return verticalWalls[0];


    }

    public GameObject chooseHorizontal()
    {
        int rand = Random.Range(0, horizontalTotal);

        GameObject result;

        for (int i = 0; i < horizontalRanges.Count; i++)
        {
            if (rand < horizontalRanges[i])
            {
                result = horizontalWalls[i];
                return result;
            }
        }
        return horizontalWalls[0] ;
    }
}
