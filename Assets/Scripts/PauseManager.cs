using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuPrefab;
    [SerializeField] private PlayerInput P1, P2;
    [SerializeField] private PauseMenu ActivePauseMenu;

    [SerializeField] private bool RequireBothPlayerstoUnpause;
    [SerializeField] private bool ForcePause;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // connect players if not all are connected
        if(P1 == null || P2 == null)
        {
            PlayerInput[] allPlayers = FindObjectsOfType<PlayerInput>();
            foreach (PlayerInput p in allPlayers)
            {
                if(p.playerIndex == 0)
                {
                    P1 = p;
                }
                else if(p.playerIndex == 1)
                {
                    P2 = p;
                }
            }
        }

        // if there are not enough players connected, force pause
        if(RequireBothPlayerstoUnpause && ActivePauseMenu == null && (P1 == null || P2 == null))
        {
            TogglePause();
            ForcePause = true;
        }
        else if (RequireBothPlayerstoUnpause && P1 != null && P2 != null)
        {
            ForcePause = false;
        }
        else if (ActivePauseMenu == null && (P1 == null && P2 == null))
        {
            TogglePause();
            ForcePause = true;
        }
        else if (!RequireBothPlayerstoUnpause && (P1 != null || P2 != null))
        {
            ForcePause = false;
        }
        // else
        // {
        //     Debug.Log("nothing to do");
        // }
    }

    public void TogglePause()
    {
        if(ActivePauseMenu != null)
        {
            if(!ForcePause)
            {
                Destroy(ActivePauseMenu.gameObject);
            }
        }
        else
        {
            ActivePauseMenu = Instantiate(PauseMenuPrefab).GetComponent<PauseMenu>();
            // TODO: consider doing additional pause stuff here
        }
    }
    
    public bool CheckIfNotUnpausable()
    {
        return ForcePause;
    }
}
