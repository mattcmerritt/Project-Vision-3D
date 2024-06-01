using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LockedInteractable : Interactable
{
    // state information to track if the item is still locked
    private bool isLocked = true;
    public bool IsLocked
    {
        get { return isLocked; }
        set { isLocked = value; }
    }

    public override void Interact(GameObject player)
    {
        if (isLocked) base.Interact(player);
    }
}
