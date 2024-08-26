using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathingNode
{
    [SerializeField] private Waypoint waypoint;
    public Waypoint Waypoint { 
        get { return this.waypoint; } 
        private set { this.waypoint = value; } 
    }

    [SerializeField] private float waitTime;
    public float WaitTime
    {
        get { return this.waitTime; }
        private set { this.waitTime = value; }
    }

    // used for establishing a linked list
    private PathingNode nextNode;
    public PathingNode NextNode
    {
        get { return this.nextNode; }
        set { this.nextNode = value; }
    }
}
