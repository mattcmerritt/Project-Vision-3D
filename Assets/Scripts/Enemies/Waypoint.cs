using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Toggles to determine how to set up the waypoint
    [SerializeField] private bool isForInteractable;
    [SerializeField] private bool isForEnemy;

    // Reference for the object that causes the enemy to walk to this point
    //  Likely useful to make the character face that direction
    [SerializeField] private Vector3 objectLocation;
    public Vector3 ObjectLocation
    {
        get { return this.objectLocation; }
        private set { this.objectLocation = value; }
    }

    // The object's true world position
    [SerializeField] private Vector3 trueLocation;
    public Vector3 TrueLocation
    {
        get { return this.trueLocation; }
        private set { this.trueLocation = value; }
    }

    private void Start()
    {
        // if this waypoint is attached to an interactable,
        //  the true location needs to be the global position of the waypoint
        //  and the object location should be the global position of the interactable object
        if (isForInteractable)
        {
            Interactable triggerObject = GetComponentInParent<Interactable>();
            TrueLocation = transform.position;
            ObjectLocation = triggerObject.transform.position;
        }

        // if this waypoint is attached to an enemy,
        //  the true location should be the global position of the waypoint
        //  the object location should be set up in the editor, otherwise it is the true location
        if (isForEnemy)
        {
            TrueLocation = transform.position;
            if (ObjectLocation == Vector3.zero)
            {
                ObjectLocation = TrueLocation;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (isForEnemy)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ObjectLocation, 0.25f);

            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(TrueLocation, 0.25f);
            }
        }
        else if (isForInteractable)
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(ObjectLocation, 0.25f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(TrueLocation, 0.25f);
            }
        }
    }
}
