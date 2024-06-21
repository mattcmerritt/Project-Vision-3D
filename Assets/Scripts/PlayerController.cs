using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement movement;

    public void Start() 
    {
        DontDestroyOnLoad(gameObject);
        AttachPlayer();
    }

    public void Update()
    {
        if(movement != null)
        {
            // second vector is offset so player is lower on screen (y) and camera can actually see scene (z)
            transform.position = movement.transform.position;
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachPlayer();
    }

    public void AttachPlayer()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        int index = GetComponent<PlayerInput>().playerIndex;
        foreach (PlayerMovement p in players)
        {
            if (p.GetIndex() == index)
            {
                movement = p;
            }
        }
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        if (movement != null)
        {
            movement.Move(context);
        }
        else
        {
            AttachPlayer(); // failsafe if player not attached yet
        }
    }

    public void TriggerInteract(InputAction.CallbackContext context)
    {
        if (movement != null)
        {
            movement.Interact(context);
        }
        else
        {
            AttachPlayer(); // failsafe if player not attached yet
        }
    }

    public void TriggerCancel(InputAction.CallbackContext context)
    {
        if (movement != null)
        {
            movement.Cancel(context);
        }
        else
        {
            AttachPlayer(); // failsafe if player not attached yet
        }
    }

    public void TriggerPause(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            PauseManager pm = FindObjectOfType<PauseManager>();
            if(pm == null)
            {
                Debug.LogError("ERROR: The pause manager does not exist!");
            }
            else if(!pm.CheckIfNotUnpausable())
            {
                pm.TogglePause();
            }
        }
    }
}
