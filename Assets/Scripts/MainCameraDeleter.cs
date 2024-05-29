using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the whole point of this script is to remove the camera once two players are in to save performance
public class MainCameraDeleter : MonoBehaviour
{
    [SerializeField] private GameObject InitialCamera;

    void Update()
    {
        if(InitialCamera.activeSelf && FindObjectsOfType<PlayerController>().Length >= 2)
        {
            InitialCamera.SetActive(false);
        }
    }
}
