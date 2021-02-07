using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles inputs and what to do with them
/// </summary>
public class InputController : MonoBehaviour
{
    //Cursor prefab.
    public GameObject Cursor;

    //Mouse positions for current-frame, previous frame, and difference between the two.
    public Vector3 CurrentMousePosition;
    private Vector3 LastMousePosition;
    private Vector3 MousePositionDifference;

    //Tracks mouse scroll ammount.
    private float MouseScroll;

    //Movement axis.
    private float MoveAxisX;
    private float MoveAxisY;

    //Controllers to manipulate.
    public CameraController CameraController;
    public BuildController BuildController;

    //Tile currently under the mouse position, sent to BuilderController.
    private Tile TileUnderMouse;

    private bool ControlPlayer;

    // Update is called once per frame
    void Update()
    {
        CurrentMousePosition = GetMousePosition();

        UpdateMouseInputs();
        UpdateKeyInputs();

        LastMousePosition = GetMousePosition();
    }


    /// <summary>
    /// Gets the mouse position in respect to the camera.
    /// </summary>
    Vector3 GetMousePosition()
    {
        Vector3 MousePosition = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);

        //Height must be nullified
        MousePosition.y = 0;

        Cursor.transform.position = CurrentMousePosition;

        return MousePosition;

    }


    /// <summary>
    /// Manages actions from any mouse input, also tracks states.
    /// </summary>
    void UpdateMouseInputs()
    {
        MouseScroll = Input.GetAxis("Mouse ScrollWheel");
        MousePositionDifference = LastMousePosition - CurrentMousePosition;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //Left-click down, the player might be beggining a selection.
        if (Input.GetMouseButtonDown(0))
        {
            BuildController.SelectionStartPosition = CurrentMousePosition;
        }
        //Left-click held, something's being previewed and prepared for building.
        if (Input.GetMouseButton(0))
        {
            if(CurrentMousePosition != LastMousePosition)
            {
                BuildController.SetSelectionVectors(CurrentMousePosition);
            }
        }

        //Left-click up, we have to determine if this was a request to build, or an interaction.
        if (Input.GetMouseButtonUp(0))
        {
            BuildController.ResolveContext();
        }

        //Middle-click is being held.
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            CameraController.DragCamera(MousePositionDifference);
        }
        //TODO: Middle-click snap is buggy ATM. How do we make it so that a single-click event isn't triggered by middle-click dragging?
        /*if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && MousePositionDifference == Vector3.zero)
        {
            CameraController.SnapCamera(CurrentMousePosition);
        }*/

        //The mouse scrollwheel has been moved.
        if (MouseScroll != 0)
        {
            CameraController.ZoomCamera(MouseScroll);
        }
    }

    /// <summary>
    /// Keeps track of keyboard inputs.
    /// </summary>
    void UpdateKeyInputs()
    {

        MoveAxisX = Input.GetAxisRaw("Horizontal");
        MoveAxisY = Input.GetAxisRaw("Vertical");

        if (!ControlPlayer)
        {
            //Give CameraController's DragCamera a vector to move towards, depending on what has been pressed.
            //These vectors have to be diminished, lest we zoom by the map extremely quickly.
            CameraController.DragCamera(new Vector3(MoveAxisX / 15, 0, MoveAxisY / 15));
        }

        //P key is for switching the camera into player mode.
        //TODO: This must change building behavior too.
        if (Input.GetKeyUp(KeyCode.P))
        {
            SetPlayMode();
        }

    }


    /// <summary>
    /// Sets the game to PLAYMODE/BUILDMODE
    /// This changes the behaviors of things like the camera, movement, UI, etc.
    /// </summary>
    public void SetPlayMode()
    {

        ControlPlayer = !ControlPlayer;
        CameraController.followPlayer = !CameraController.followPlayer;

        //We have to nullify these, to prevent the player/camera from continuing to move, if the mode is switched suddenly.
        //PlayerController.MoveX = MoveAxisX = 0;
        //PlayerController.MoveY = MoveAxisY = 0;

    }


    public void Quit()
    {
        Application.Quit();
    }
}
