using System;
using UnityEngine;

public class Node : IEquatable<Node>
{
    public Vector3Int position;
    public int cost;
    public int heuristic { get; set; }
    public Node parent;

    public Node(Vector3Int pos, int cost, Node parent)
    {
        this.position = pos;
        this.cost = cost;
        this.parent = parent;
    }

    public bool Equals(Node other)
    {
        if (other == null)
            return false;
        return position.x == other.position.x &&
               position.y == other.position.y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        Node objAsNode = obj as Node;
        if (objAsNode == null)
            return false;
        else
            return Equals(objAsNode);
    }

    public override int GetHashCode()
    {
        return position.GetHashCode() ^ cost.GetHashCode() ^ parent.GetHashCode();
    }
}
