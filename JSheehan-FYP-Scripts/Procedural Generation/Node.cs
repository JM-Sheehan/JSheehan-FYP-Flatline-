using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Node
{
    // Each Node bar the nodes at the lowest level will have a list of children nodes
    private List<Node> childrenNodes;

    public List<Node> ChildrenNodes { get => childrenNodes; }
    // Used when traversing the tree
    public bool Visted { get; set; }

    // Coordinates for mapping to the given node.
    public Vector2Int BLCorner { get; set; }
    public Vector2Int BRCorner { get; set; }
    public Vector2Int TLCorner { get; set; }
    public Vector2Int TRCorner { get; set; }

    public Node ParentNode { get; set; }

    // Level in the binary tree.
    public int TreeLayerIndex { get; set; }

    public Node(Node parentNode)
    {
        childrenNodes = new List<Node>();
        this.ParentNode = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    public void AddChild(Node node)
    {
        childrenNodes.Add(node);

    }

    public void RemoveChild(Node node)
    {
        childrenNodes.Remove(node);
    }
}