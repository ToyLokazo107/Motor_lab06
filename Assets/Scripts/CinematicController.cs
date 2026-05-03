using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CinematicController : MonoBehaviour
{
    public List<CinemachineCamera> CinematicCameras;
    public int currentCinematicCamera;


    public CinemachineCamera FollowCamera1;
    public CinemachineCamera FollowCamera2;
    public CinemachineCamera PlayerCam;
    void Start()
    {
        InvokeRepeating(nameof(SwitchCamera), 0f, 15f);
    }
    [Button]
    public void SwitchCamera()
    {
        bool activateCamera = Random.Range(0f, 1f) > 0.5f;
        int currentCinematicCamera = Random.Range(0, CinematicCameras.Count);

        for (int i = 0; i < CinematicCameras.Count; i++)
        {
            if (i == currentCinematicCamera && activateCamera)
                CinematicCameras[i].Priority = 11;
            else
                CinematicCameras[i].Priority = 0;
        }
       
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            float distanceToA = Vector3.Distance(PlayerCam.transform.position, FollowCamera1.transform.position);
            float distanceToB = Vector3.Distance(PlayerCam.transform.position, FollowCamera2.transform.position);

            if(distanceToA < distanceToB)
            {
                FollowCamera1.Priority = 12;
                FollowCamera2.Priority = 0;
                PlayerCam.Priority = 0;
            }
            else
            {
                FollowCamera1.Priority = 0;
                FollowCamera2.Priority = 12;
                PlayerCam.Priority = 0;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {

           FollowCamera1.Priority = 0;
           FollowCamera2.Priority = 0;
           PlayerCam.Priority =10;
         
        }
    }

}
