using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Reference for the object that causes the enemy to walk to this point
    //  Likely useful to make the character face that direction
    private Vector3 objectLocation;
    public Vector3 ObjectLocation
    {
        get { return this.objectLocation; }
        private set { this.objectLocation = value; }
    }

    // The object's true world position
    private Vector3 trueLocation;
    public Vector3 TrueLocation
    {
        get { return this.trueLocation; }
        private set { this.trueLocation = value; }
    }

    private void Start()
    {
        Interactable triggerObject = GetComponentInParent<Interactable>();
        TrueLocation = transform.position;
        ObjectLocation = triggerObject.transform.position;
    }
}
