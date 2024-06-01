using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LockedDoor : LockedInteractable
{
    [SerializeField] private GameObject interactUI, lockpickingUI;
    [SerializeField] private RectTransform unlockHandleTransform, startHandleTransform, endHandleTransform;
    [SerializeField] private float failWindow, successWindow;

    // boolean flag toggled when the timer is in the right window
    // used to determine if a key press results in a success
    private bool unlockAllowed;

    // reference to the current cycle coroutine
    private Coroutine activeLockpickCycle;

    // action callbacks to detect successes and failures
    public Action onSuccess, onFailure;

    private void Start()
    {
        // set up the lockpicking UI with the proper spacing
        // TODO: reconfigure to make the success window always either the top or bottom of the wheel
        float totalDuration = failWindow + successWindow;
        startHandleTransform.rotation = Quaternion.Euler(0, 0, 0);
        endHandleTransform.rotation = Quaternion.Euler(0, 0, -failWindow / totalDuration * 360);

        // subscribe door to success action
        onSuccess += OpenDoor;
    }

    public override void InteractBehaviour(GameObject player)
    {
        // swap active UI
        interactUI.SetActive(false);
        lockpickingUI.SetActive(true);

        // prevent player from moving
        player.GetComponent<PlayerMovement>().MovementLocked = true;
        
        // begin the quick time event / minigame
        activeLockpickCycle = StartCoroutine(UnlockMinigameCycle());
    }

    public override void StopInteractBehaviour(GameObject player)
    {
        // swap active UI
        lockpickingUI.SetActive(false);
        interactUI.SetActive(true);

        // cancel the quick time event / minigame
        StopCoroutine(activeLockpickCycle);
    }

    public override void PlayerEntersProximityBehaviour()
    {
        if (IsLocked)
        {
            interactUI.SetActive(true);
        }
    }

    public override void PlayerLeavesProximityBehaviour()
    {
        interactUI.SetActive(false);
    }

    private IEnumerator UnlockMinigameCycle()
    {
        float totalDuration = failWindow + successWindow;
        float elapsedTime = 0f;
        while (elapsedTime < failWindow)
        {
            unlockAllowed = false;
            elapsedTime += Time.deltaTime;
            unlockHandleTransform.rotation = Quaternion.Euler(0, 0, -elapsedTime / totalDuration * 360);
            yield return null;
        }
        while (elapsedTime < totalDuration)
        {
            unlockAllowed = true;
            elapsedTime += Time.deltaTime;
            unlockHandleTransform.rotation = Quaternion.Euler(0, 0, -elapsedTime / totalDuration * 360);
            yield return null;
        }
        unlockHandleTransform.rotation = Quaternion.Euler(0, 0, 0);

        // repeat the cycle
        activeLockpickCycle = StartCoroutine(UnlockMinigameCycle());
    }

    public bool AttemptUnlock(GameObject player)
    {
        if (unlockAllowed)
        {
            IsLocked = false;

            player.GetComponent<PlayerMovement>().MovementLocked = false;
            
            StopInteractBehaviour(player);
            PlayerLeavesProximityBehaviour();

            onSuccess?.Invoke();
        }
        else
        {
            player.GetComponent<PlayerMovement>().MovementLocked = false;

            StopInteractBehaviour(player);

            onFailure?.Invoke();
        }
        return unlockAllowed;
    }

    // TODO: add an animator or something to make this more visually appealing
    public void OpenDoor()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}
