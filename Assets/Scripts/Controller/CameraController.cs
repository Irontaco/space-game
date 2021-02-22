using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Main Camera
    public Camera MainCamera;

    //Loose coordinate to be used for camera adjustments.
    public Vector3 CameraPosition;

    //Player instance for tracking
    public GameObject PlayerInstance;

    //Tracks if the camera is in EDIT/PLAY mode.
    public bool followPlayer = false;

    void Start()
    {
        MainCamera.transform.position = new Vector3(WorldMap.Current.Width / 2, 10, WorldMap.Current.Height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        CameraPosition = MainCamera.transform.position;
        
        //PLAY-MODE CAMERA
        if (followPlayer)
        {
            SnapToPlayer();
        }

    }

    /// <summary>
    /// Snaps the camera position to the position of the playerInstance.
    /// </summary>
    public void SnapToPlayer()
    {

        CameraPosition = PlayerInstance.transform.position;
        //We gotta keep the camera 'above' everything else in the z-plane.
        //TODO: find some way to programatically set this up.

        //Aim for the head!
        CameraPosition.y += 1;
        MainCamera.transform.position = CameraPosition;

    }

    /// <summary>
    /// Manages Camera Dragging.
    /// </summary>
    public void DragCamera(Vector3 moveAmmount)
    {
        //If we're following the player, we cannot allow camera-dragging.
        if (followPlayer)
        {
            return;
        }

        //Otherwise, let's move the camera over.
        MainCamera.transform.position += moveAmmount;
    }

    /// <summary>
    /// Snaps the camera to an specific vector position.
    /// </summary>
    public void SnapCamera(Vector3 position)
    {
        MainCamera.transform.position = position;
    }

    /// <summary>
    /// Updates the camera's ortkographic size depending on what the scrollwheel input returns.
    /// </summary>
    public void ZoomCamera(float ZoomAmmount)
    {

        MainCamera.orthographicSize -= (Screen.height / (32 * 1) * ZoomAmmount);

        //Clamp it, this sets the minimum-maximum orthographic size.
        MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize, 1f, 70f);
    }
}
