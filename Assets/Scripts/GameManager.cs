using System;
using Unity.Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    public CinemachineCamera camA;
    public CinemachineCamera camB;


    [Button("Transition")]
    public void transition()
    {
        if (camA.Priority > camB.Priority)
        {
            camA.Priority = 0;
            camB.Priority = 1;
        }
        else
        {
            camA.Priority = 1;
            camB.Priority = 0;
        }
    }
    public void OnCameraFinished()
    {
        Debug.Log("Camera transition finished!");
    }
}
