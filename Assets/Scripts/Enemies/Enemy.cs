using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // motion details
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Waypoint currentWaypoint, previousWaypoint;
    [SerializeField] private Vector3 currentDirection = Vector3.right;
    private float speed = 5;
    private Coroutine waitForArrivalCoroutine;

    // vision cone details
    private float viewDistance = 15;

    // detected objects
    private HashSet<GameObject> detectedInteractables = new HashSet<GameObject>();

    // suspicion and behaviour information
    [SerializeField] private List<Suspicion> suspicions = new List<Suspicion>();
    private Dictionary<Interactable, Coroutine> currentlySuspiciousActions = new Dictionary<Interactable, Coroutine>();

    // detecting in front of the player
    private void Update()
    {
        // vision cone detection
        //  uses cross products to determine if the object falls between two lines
        //  linear algebra is explained here: https://stackoverflow.com/questions/45766534/finding-cross-product-to-find-points-above-below-a-line-in-matplotlib
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, viewDistance);

        // reset the currently observed objects
        HashSet<GameObject> previouslyFound = detectedInteractables;
        HashSet<GameObject> found = new HashSet<GameObject>();

        foreach (Collider nearbyObject in nearbyObjects)
        {
            if (nearbyObject.GetComponent<Interactable>())
            {
                Vector3 upperAngle = Quaternion.Euler(0, 30, 0) * currentDirection;
                Vector3 lowerAngle = Quaternion.Euler(0, -30, 0) * currentDirection;

                Vector3 upperEndpoint = transform.position + upperAngle * viewDistance;
                Vector3 lowerEndpoint = transform.position + lowerAngle * viewDistance;

                float upperCross = LinearMath.Cross2D(nearbyObject.transform.position, transform.position, upperEndpoint);
                float lowerCross = LinearMath.Cross2D(nearbyObject.transform.position, transform.position, lowerEndpoint);

                if (upperCross < 0 && lowerCross > 0)
                {
                    // check if the object was already seen
                    if (found.Add(nearbyObject.gameObject))
                    {
                        Debug.Log($"<color=blue>Collision: </color> {nearbyObject.name} is seen by {gameObject.name}.");
                    }
                    else
                    {
                        Debug.Log($"<color=blue>Collision: </color> {nearbyObject.name} is still seen by {gameObject.name}.");
                    }
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

        #region Player Detaining
        /*
        // scanning directly in front of the enemies
        RaycastHit[] hits = Physics.RaycastAll(transform.position, currentDirection, viewDistance);

        HashSet<PlayerMovement> playersDetainedThisCycle = new HashSet<PlayerMovement>();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<PlayerMovement>() && !hit.collider.GetComponent<PlayerMovement>().Hidden)
            {
                // previously would restart the level on being caught
                // GameManager.instance.FailAndRestartLevel();

                // now, the player is held in place and cannot move
                playersDetainedThisCycle.Add(hit.collider.GetComponent<PlayerMovement>());
            }
        }

        // detaining all caught players to restrict movement
        foreach (PlayerMovement player in playersDetainedThisCycle)
        {
            player.Detained = true;
        }
        // releasing any players from last cycle that were not detained this cycle
        foreach (PlayerMovement player in playersDetained)
        {
            if (!playersDetainedThisCycle.Contains(player))
            {
                player.Detained = false;
            }
        }
        // setting the players detained to the players detained at the end of this frame
        playersDetained = playersDetainedThisCycle;
        */
        #endregion Player Detaining
    }

    // DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + currentDirection * viewDistance);

        Gizmos.color = Color.green;
        Vector3 angle1 = Quaternion.Euler(0, 30, 0) * currentDirection;
        Vector3 angle2 = Quaternion.Euler(0, -30, 0) * currentDirection;

        Gizmos.DrawLine(transform.position, transform.position + angle1 * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + angle2 * viewDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }

    public void SetTargetWaypoint(Waypoint newWaypoint)
    {
        // do not allow moving to the same point
        if (newWaypoint == currentWaypoint) return;

        previousWaypoint = currentWaypoint;
        currentWaypoint = newWaypoint;

        // move between positions using nav agent
        navAgent.SetDestination(currentWaypoint.TrueLocation);
        waitForArrivalCoroutine = StartCoroutine(WaitToLookAtDestination(currentWaypoint));
    }

    private IEnumerator WaitToLookAtDestination(Waypoint target)
    {
        while ((transform.position - target.TrueLocation).magnitude < 0.1f)
        {
            // calculate and adjust direction
            currentDirection = Vector3.Normalize(target.TrueLocation - transform.position);
            transform.LookAt(target.TrueLocation);

            yield return null;
        }

        transform.LookAt(target.ObjectLocation);
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
            SetTargetWaypoint(source.waypoint);
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
