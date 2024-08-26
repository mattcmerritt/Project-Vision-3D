using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathingMap
{
    // Internal structure for showing which nodes are present in this map
    //  Should only be set and used in the editor.
    //  All other operations should take advantage of the linked list formed by the nodes.
    [SerializeField] private List<PathingNode> nodes;

    // Reference to keep track of where we are in the linked list of nodes.
    private PathingNode mostRecentNode;
    
    // Use the list of nodes to form a circular linked list.
    public void Initialize()
    {
        if (nodes == null || nodes.Count == 0) Debug.LogWarning("Pathing map could not be built!");

        for (int i = 0; i < nodes.Count; i++)
        {
            if (i < nodes.Count - 1)
            {
                nodes[i].NextNode = nodes[i + 1];
            }
            else
            {
                nodes[i].NextNode = nodes[0];
            }
        }

        mostRecentNode = nodes[0];
    }

    public PathingNode GetNextPathingNode()
    {
        mostRecentNode = mostRecentNode.NextNode;
        return mostRecentNode;
    }

    public PathingNode GetCurrentPathingNode()
    {
        return mostRecentNode;
    }
}
