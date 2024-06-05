using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // motion details
    [SerializeField] private Waypoint currentWaypoint, previousWaypoint;
    private Vector3 currentDirection = Vector3.right;
    private float speed = 5;

    // current motion reference
    private Coroutine movementCoroutine;

    // vision cone details
    [SerializeField] private GameObject visionConeObject;
    private float viewDistance = 5;

    // detained player details
    private HashSet<PlayerMovement> playersDetained = new HashSet<PlayerMovement>();

    // detecting in front of the player
    private void Update()
    {
        // scanning in front of the enemies in the vision cone
        RaycastHit[] hits = Physics.RaycastAll(transform.position + currentDirection * 0.5f, currentDirection, viewDistance);

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
    }

    // DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + currentDirection * 0.5f, transform.position + currentDirection * 0.5f + currentDirection * viewDistance);
    }

    public void SetTargetWaypoint(Waypoint newWaypoint)
    {
        // do not allow moving to the same point
        if (newWaypoint == currentWaypoint) return;

        previousWaypoint = currentWaypoint;
        currentWaypoint = newWaypoint;

        // calculate direction
        currentDirection = Vector3.Normalize(currentWaypoint.TrueLocation - previousWaypoint.TrueLocation);

        // move between positions using coroutine
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(MoveToPosition(newWaypoint.TrueLocation));
    }

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
            yield return null;
        }

        // move the enemy exactly to the end
        transform.position = target;
    }
}
