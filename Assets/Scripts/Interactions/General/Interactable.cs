using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interactable : MonoBehaviour
{
    // tracking if players are near the object
    private HashSet<GameObject> playersClose = new HashSet<GameObject>();

    // adding a interaction type to group this interaction by for suspicion purposes
    public InteractionTypes interactionType { get; protected set; }

    // events for enemies to latch onto when in detection range
    public Action<Interactable> onPlayerStartInteract, onPlayerStopInteract;

    // waypoint information for enemies that are suspicious of this interaction
    public Waypoint waypoint { get; private set; }

    // fetch the waypoint for enemy navigation
    protected virtual void Start()
    {
        waypoint = GetComponentInChildren<Waypoint>();
    }

    public virtual void Interact(GameObject player)
    {
        InteractBehaviour(player);
        onPlayerStartInteract?.Invoke(this);
    }

    public virtual void StopInteract(GameObject player)
    {
        StopInteractBehaviour(player);
        onPlayerStopInteract?.Invoke(this);
    }

    public abstract void InteractBehaviour(GameObject player);
    public abstract void StopInteractBehaviour(GameObject player);

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>())
        {
            if (playersClose.Count == 0)
            {
                PlayerEntersProximityBehaviour();
            }
            playersClose.Add(other.gameObject);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>())
        {
            playersClose.Remove(other.gameObject);
            if (playersClose.Count == 0)
            {
                PlayerLeavesProximityBehaviour();
            }
        }
    }

    // overwritable methods to allow for custom behaviors when a player gets close
    // likely will only involve activating the interaction UI
    public abstract void PlayerEntersProximityBehaviour();
    public abstract void PlayerLeavesProximityBehaviour();
}
