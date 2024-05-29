using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 direction;

    // interaction details
    // TODO: reenable
    // private InteractableElement currentElement;
    private bool movementLocked;
    public bool MovementLocked
    {
        get { return movementLocked; }
        set { movementLocked = value; }
    }
    private bool hidden;
    public bool Hidden
    {
        get { return hidden; }
        set { hidden = value; }
    }

    // detainment details
    private bool detained;
    public bool Detained
    {
        get { return detained; }
        set { detained = value; }
    }

    // storing reference to the last interactable element that the player is close to
    private void OnTriggerEnter(Collider collision)
    {
        // TODO: reenable
        // if (collision.gameObject.GetComponent<InteractableElement>())
        // {
        //     currentElement = collision.gameObject.GetComponent<InteractableElement>();
        // }
    }

    // clearing references to objects if player gets too far away
    private void OnTriggerExit(Collider collision)
    {
        // TODO: reenable
        // if (collision.gameObject.GetComponent<InteractableElement>() == currentElement)
        // {
        //     currentElement = null;
        // }
    }

    private void FixedUpdate()
    {
        // check to see if unpaused
        if(FindObjectOfType<PauseMenu>() == null)
        {
            if (!movementLocked && !detained)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(direction.x * speed, GetComponent<Rigidbody>().velocity.y, direction.y * speed);
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        // ignore inputs if paused
        if(FindObjectOfType<PauseMenu>() == null)
        {
            if(context.started)
            {
                // Debug.Log("started");
            }
            else if(context.performed)
            {
                // Debug.Log("performed");
                direction = context.ReadValue<Vector2>();
            }
            else if(context.canceled)
            {
                // Debug.Log("canceled");
                direction = Vector2.zero;
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        // ignore inputs if paused
        if(FindObjectOfType<PauseMenu>() == null)
        {
            // TODO: reenable
            // if (currentElement == null) return;

            // // handling interact presses in an interact popup
            // if (movementLocked)
            // {
            //     if (currentElement is LockedDoor)
            //     {
            //         if (context.started)
            //         {
            //             LockedDoor lockedDoor = (LockedDoor) currentElement;
            //             lockedDoor.AttemptUnlock(gameObject);
            //         }
            //     }
            //     if (currentElement is Cover)
            //     {
            //         if (context.started)
            //         {
            //             Cover cover = (Cover) currentElement;
            //             cover.Reveal(gameObject);
            //         }
            //     }
            // }
            // // handling interact presses outside of popups
            // else
            // {
            //     if (context.started)
            //     {
            //         // Debug.Log("started");
            //         Debug.Log("interact pressed");
            //         currentElement.Interact(gameObject);
            //     }
            //     else if (context.performed)
            //     {
            //         // Debug.Log("performed");

            //     }
            //     else if (context.canceled)
            //     {
            //         // Debug.Log("canceled");
            //     }
            // } 
        }
    }

    public int GetIndex()
    {
        return index;
    }
}
