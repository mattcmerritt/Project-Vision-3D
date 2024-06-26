using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // motion details
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Waypoint currentWaypoint, previousWaypoint;
    private Coroutine waitForArrivalCoroutine;

    // vision cone details
    private float viewDistance = 15;

    // detected objects
    private HashSet<GameObject> detectedInteractables = new HashSet<GameObject>();

    // suspicion and behaviour information
    [SerializeField] private List<Suspicion> suspicions = new List<Suspicion>();
    private Dictionary<Interactable, Coroutine> currentlySuspiciousActions = new Dictionary<Interactable, Coroutine>();

    // patrolling and passive movement information
    [SerializeField] private PathingMap pathingMap;
    private PathingNode currentPathingStep;
    private float timeSpentInPathingStep;
    private bool isCurrentlyUsingPathingMap = true;

    private void Start()
    {
        pathingMap.Initialize();
        currentPathingStep = pathingMap.GetCurrentPathingNode();
    }

    // detecting in front of the player
    private void Update()
    {
        #region Vision Cone Detection
        //  uses cross products to determine if the object falls between two lines
        //  linear algebra is explained here: https://stackoverflow.com/questions/45766534/finding-cross-product-to-find-points-above-below-a-line-in-matplotlib
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, viewDistance);

        // reset the currently observed objects
        HashSet<GameObject> previouslyFound = detectedInteractables;
        HashSet<GameObject> found = new HashSet<GameObject>();

        foreach (Collider nearbyObject in nearbyObjects)
        {
            if (!nearbyObject.isTrigger) continue;

            if (nearbyObject.GetComponent<Interactable>())
            {
                Vector3 upperAngle = Quaternion.Euler(0, 30, 0) * transform.forward;
                Vector3 lowerAngle = Quaternion.Euler(0, -30, 0) * transform.forward;

                Vector3 upperEndpoint = transform.position + upperAngle * viewDistance;
                Vector3 lowerEndpoint = transform.position + lowerAngle * viewDistance;

                float upperCross = LinearMath.Cross2D(nearbyObject.transform.position, transform.position, upperEndpoint);
                float lowerCross = LinearMath.Cross2D(nearbyObject.transform.position, transform.position, lowerEndpoint);

                if (upperCross < 0 && lowerCross > 0)
                {
                    // check if the object was already seen
                    found.Add(nearbyObject.gameObject);
                    Debug.Log($"<color=blue>Collision: </color> {nearbyObject.name} is seen by {gameObject.name}.");
                }
            }
        }

        // subscribe to the interaction trigger events for any observable interactions that were added this cycle
        foreach (GameObject detectedObject in found)
        {
            if (!previouslyFound.Contains(detectedObject))
            {
                Interactable detectedInteractable = detectedObject.GetComponent<Interactable>();
                detectedInteractable.onPlayerStartInteract += StartSuspiciousAction;
                detectedInteractable.onPlayerStopInteract += CeaseSuspiciousAction;
            }
        }

        // overwrite the list of detected interactables
        detectedInteractables = found;
        #endregion Vision Cone Detection

        #region Pathing Map Updates
        if (isCurrentlyUsingPathingMap)
        {
            timeSpentInPathingStep += Time.deltaTime;
            if (timeSpentInPathingStep > currentPathingStep.WaitTime)
            {
                currentPathingStep = pathingMap.GetNextPathingNode();
                timeSpentInPathingStep = 0f;

                SetTargetWaypoint(currentPathingStep.Waypoint, false);
            }
        }
        #endregion Pathing Map Updates
    }

    // DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * viewDistance);

        Gizmos.color = Color.green;
        Vector3 angle1 = Quaternion.Euler(0, 30, 0) * transform.forward;
        Vector3 angle2 = Quaternion.Euler(0, -30, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + angle1 * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + angle2 * viewDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // DIRECTIONAL TESTS
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + transform.forward, 0.25f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.right, 0.25f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + transform.up, 0.25f);
    }

    public void SetTargetWaypoint(Waypoint newWaypoint, bool isDistraction)
    {
        // do not allow moving to the same point
        if (newWaypoint == currentWaypoint) return;

        previousWaypoint = currentWaypoint;
        currentWaypoint = newWaypoint;

        // move between positions using nav agent
        navAgent.SetDestination(LinearMath.GetSameHeightPoint(transform.position, currentWaypoint.TrueLocation));
        waitForArrivalCoroutine = StartCoroutine(WaitToLookAtDestination(currentWaypoint, isDistraction));
    }

    private IEnumerator WaitToLookAtDestination(Waypoint target, bool isDistraction)
    {
        // while in motion, look at the target object
        while (navAgent.remainingDistance > 0f)
        {
            transform.LookAt(LinearMath.GetSameHeightPoint(transform.position, target.ObjectLocation));
            yield return null;
        }

        // if this is a distraction from an interactable, the enemy needs to return to standard pathing
        if (isDistraction)
        {
            StartCoroutine(ReturnToPathingMap());
        }
        // otherwise, once the enemy is back at their waypoint, mark as still using the navigation map
        else
        {
            isCurrentlyUsingPathingMap = true;
        }
    }

    private IEnumerator ReturnToPathingMap()
    {
        // wait a delay before returning to the standard behavior
        yield return new WaitForSeconds(3);

        // set the player back on their original path
        SetTargetWaypoint(currentPathingStep.Waypoint, false);
    }

    private void StartSuspiciousAction(Interactable source)
    {
        currentlySuspiciousActions.Add(source, StartCoroutine(GrowSuspicion(source)));
    }

    private IEnumerator GrowSuspicion(Interactable source)
    {
        Suspicion associatedSuspicion = suspicions.Find((Suspicion s) => s.interactionType == source.interactionType);

        while (associatedSuspicion != null && associatedSuspicion.currentLevel < associatedSuspicion.threshold)
        {
            associatedSuspicion.currentLevel += Time.deltaTime * associatedSuspicion.rateOfIncrease;
            yield return null;
        }

        if (associatedSuspicion.currentLevel >= associatedSuspicion.threshold)
        {
            isCurrentlyUsingPathingMap = false;
            SetTargetWaypoint(source.waypoint, true);
        }
    }

    private void CeaseSuspiciousAction(Interactable source)
    {
        currentlySuspiciousActions.TryGetValue(source, out Coroutine suspicionCoroutine);
        StopCoroutine(suspicionCoroutine);
        currentlySuspiciousActions.Remove(source);
    }

    #region Waypoint Movement (Old Interpolation)
    /*
    private IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        Vector3 totalDistance = startPosition - target;
        float duration = totalDistance.magnitude / speed;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
            transform.eulerAngles = new Vector3(90f, 0f, 0f);
            yield return null;
        }

        // move the enemy exactly to the end
        transform.position = target;
    }
    */
    #endregion Waypoint Movement (Old Interpolation)
}
