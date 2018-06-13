using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;


/// <summary>
/// Camera Controller for a Manual Movement Testing Camera
/// </summary>
public class ManualCameraMovement : DefaultCameraMovement {

    ///////////////////////////////////////
    ///         Public Variables        ///
    ///////////////////////////////////////

    /// <summary>
    /// Input to move camera
    /// </summary>
    [Tooltip("Input to move camera")]
    public Vector3Action cameraInput;
    /// <summary>
    /// Input to move look at point
    /// </summary>
    [Tooltip("Input to move look at point")]
    public Vector3Action lookAtInput;
    /// <summary>
    /// First button to toggle camera on
    /// </summary>
    [Tooltip("First button to toggle camera on")]
    public ButtonAction CameraToggle1;
    /// <summary>
    /// Second button to toggle camera on
    /// </summary>
    [Tooltip("Second button to toggle camera on")]
    public ButtonAction CameraToggle2;
    /// <summary>
    /// Offset starting position 
    /// </summary>
    [Tooltip("Offset starting position")]
    public Vector3 OriginOffset;
    /// <summary>
    /// Toggle whether camera is on and off at the start
    /// </summary>
    [Tooltip("Toggle whether camera is on and off at the start")]
    public bool StartOn;
    /// <summary>
    /// Toggle whether camera should move with animal
    /// </summary>
    [Tooltip("Toggle whether camera should move with animal")]
    public bool FollowAnimal;
    /// <summary>
    /// Speed of camera movement
    /// </summary>
    [Tooltip("Speed of camera movement")]
    public float MovementSpeed;
    /// <summary>
    /// Speed of look at point movement
    /// </summary>
    [Tooltip("Speed of look at point movement")]
    public float LookSpeed;
    /// <summary>
    /// Changes smoothness of the movement of the camera
    /// </summary>
    [Tooltip("Changes smoothness of the movement of the camera")]
    public float SmoothingSpeed;

    ///////////////////////////////////////
    ///         Private Variables       ///
    ///////////////////////////////////////

    /// <summary>
    /// Stores Original Position
    /// </summary>
    private Vector3 OriginalPosition;
    /// <summary>
    /// Stores Original Lookat Position
    /// </summary>
    private Vector3 OriginalLookat;
    /// <summary>
    /// Offset of Camera Movement Inputs
    /// </summary>
    private Vector3 CameraOffset;
    /// <summary>
    /// Offset of Look at Inputs
    /// </summary>
    private Vector3 LookatOffset;
    /// <summary>
    /// Offset based of Animal Movement
    /// </summary>
    private Vector3 AnimalOffset;
    /// <summary>
    /// Is this camera on?
    /// </summary>
    private bool CameraActivated = false;
    /// <summary>
    /// Stops camera from continuously toggling on and off
    /// </summary>
    private bool doneToggle;
    /// <summary>
    /// preserves StartOn for first frame
    /// </summary>
    private bool doneStart;
    /// <summary>
    /// has FollowAnimal been switched while playing
    /// </summary>
    private bool ToggleFollow;
    /// <summary>
    /// Transform of look at point
    /// </summary>
    private Transform LookAtTransform;
    
    // Initialisation
	protected override void OnStart () {
        //Bind inputs from InputManager
        cameraInput.Bind(GameManager.Instance.input.handle);
        lookAtInput.Bind(GameManager.Instance.input.handle);
        CameraToggle1.Bind(GameManager.Instance.input.handle);
        CameraToggle2.Bind(GameManager.Instance.input.handle);

        //Set origins and initial offsets
        CameraOffset = transform.position + OriginOffset;
        CurrentPoint = CameraOffset;
        LookAtTransform = transform.GetChild(0).transform;
        OriginalLookat = LookAtTransform.position - transform.position;

        //StartOn initialisation
        if (StartOn)
        {
            doneStart = true;
            CameraSplineManager.instance.OverrideSpline = this;
        }
        
	}

    /// <summary>
    /// Calculate position the camera should be in
    /// </summary>
    protected override void CalcPosition () {
        AnimalOffset = currentAnimal.transform.position;
        if (CameraActivated)
        {
            CameraOffset += cameraInput.control.vector3 * MovementSpeed;
            LookatOffset += lookAtInput.control.vector3 * LookSpeed;
            AnimalOffset += CameraOffset;

            if (LookAtTransform != null)
                SetCurrentPoint(AnimalOffset);
        }
        else
        {
            OriginalPosition = CurrentPoint;
        }
    }

    protected override void OnUpdate()
    {
        //Activates and Disactivates Camera
        if ((CameraToggle1.control.isHeld && CameraToggle2.control.isHeld && !doneToggle) || doneStart)
        {
            if (doneStart)
            {
                CameraActivated = true;
            }
            else
            {
                CameraActivated = !CameraActivated;
            }
            doneStart = false;
            doneToggle = true;
            if (CameraActivated)
            {
                SetAnimalEnabled(ANIMAL_NAME.LORIS, true);
                SetAnimalEnabled(ANIMAL_NAME.POODLE, true);
                SetAnimalEnabled(ANIMAL_NAME.ANTEATER, true);
                SetAnimalEnabled(ANIMAL_NAME.ZEBRA, true);
                SetAnimalEnabled(ANIMAL_NAME.ELEPHANT, true);

                CameraSplineManager.instance.OverrideSpline = this;
            }
            else
            {
                SetAnimalEnabled(ANIMAL_NAME.LORIS, false);
                SetAnimalEnabled(ANIMAL_NAME.POODLE, false);
                SetAnimalEnabled(ANIMAL_NAME.ANTEATER, false);
                SetAnimalEnabled(ANIMAL_NAME.ZEBRA, false);
                SetAnimalEnabled(ANIMAL_NAME.ELEPHANT, false);

                CameraSplineManager.instance.OverrideSpline = null;
            }
        }
        else
        {
            doneToggle = false;
        }

        //preserves position if FollowAnimal is toggled
        if(FollowAnimal != ToggleFollow && currentAnimal != null)
        {
            if (FollowAnimal)
            {
                CameraOffset -= currentAnimal.transform.position;
            }
            else
            {
                CameraOffset += currentAnimal.transform.position;
            }
            ToggleFollow = FollowAnimal;
        }
    }

    /// <summary>
    /// Tells CaneraSplineManager where to move camera to.
    /// </summary>
    /// <param name="data"></param>
    public override void SetCurrentPoint(object data)
    {
        if (data.GetType() != typeof(Vector3))
            return;

        Vector3 AnimalOffset = (Vector3)data;
        CurrentPoint = FollowAnimal ? OriginalPosition + AnimalOffset : OriginalPosition + CameraOffset;
        LookAtTransform.position = OriginalLookat + CurrentPoint + LookatOffset;
        LookAtPoint = LookAtTransform.position;
    }
}
