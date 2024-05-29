using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Image P1Panel, P2Panel;
    [SerializeField] private TMP_Text P1ConnectionStatus, P2ConnectionStatus;
    [SerializeField] private GameObject UnpauseInstructions;

    [SerializeField] private Color ConnectedColor, DisconnectedColor;

    private PlayerInput P1, P2;

    private void Update()
    {
        PlayerInput[] allPlayers = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput p in allPlayers)
        {
            if(p.playerIndex == 0)
            {
                P1 = p;
                UpdateP1Connection();
            }
            else if(p.playerIndex == 1)
            {
                P2 = p;
                UpdateP2Connection();
            }
        }

        if(P1 != null && P2 != null)
        {
            UnpauseInstructions.SetActive(true);
        }
        else
        {
            UnpauseInstructions.SetActive(false);
        }
    }

    public void UpdateP1Connection()
    {
        P1Panel.color = ConnectedColor;
        P1ConnectionStatus.text = "Connected";
    }

    public void UpdateP2Connection()
    {
        P2Panel.color = ConnectedColor;
        P2ConnectionStatus.text = "Connected";
    }
}
