using UnityEngine;
public class Room : Node
{


    // Class for holding the regular node info as well as the nodes width and length.
    public Room(Vector2Int bLCorner, Vector2Int tRCorner, Node parentNode, int index) : base(parentNode)
    {
        this.BLCorner = bLCorner;
        this.TRCorner = tRCorner;
        this.BRCorner = new Vector2Int(tRCorner.x, bLCorner.y);
        this.TLCorner = new Vector2Int(bLCorner.x, tRCorner.y);
        this.TreeLayerIndex = index;

    }

    public int Width { get => (int)(TRCorner.x - BLCorner.x); }
    public int Length { get => (int)(TRCorner.y - BLCorner.y); }

}