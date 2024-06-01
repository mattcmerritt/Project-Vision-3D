using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interactable : MonoBehaviour
{
    // tracking if players are near the object
    private HashSet<GameObject> playersClose = new HashSet<GameObject>();

    // action callback to allow enemies to listen for changes
    public Action onInteractedWith;

    public virtual void Interact(GameObject player)
    {
        InteractBehaviour(player);
        onInteractedWith?.Invoke();
    }

    public virtual void StopInteract(GameObject player)
    {
        StopInteractBehaviour(player);
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
