using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class ManualCameraMovement : DefaultCameraMovement {

    private Vector3 OriginalPosition;
    private Vector3 OriginalLookat;
    private Vector3 CameraOffset;
    private Vector3 LookatOffset;
    private bool CameraActivated = false;
    private bool doneToggle;
    private bool doneStart;
    private Transform LookAtTransform;

    public Vector3Action cameraInput;
    public Vector3Action lookAtInput;
    public ButtonAction CameraToggle1;
    public ButtonAction CameraToggle2;
    public Vector3 OriginOffset;
    public bool StartOn;
    public bool FollowAnimal;
    public float MovementSpeed;
    public float LookSpeed;
    public float SmoothingSpeed;
	// Use this for initialization
	protected override void OnStart () {
        OriginalPosition = transform.position + OriginOffset;
        cameraInput.Bind(GameManager.Instance.input.handle);
        lookAtInput.Bind(GameManager.Instance.input.handle);
        CameraToggle1.Bind(GameManager.Instance.input.handle);
        CameraToggle2.Bind(GameManager.Instance.input.handle);
        LookAtTransform = transform.GetChild(0).transform;
        OriginalLookat = LookAtTransform.position - transform.position;
        doneStart = StartOn;
        if(StartOn)
        {
            CameraSplineManager.instance.OverrideSpline = this;
        }
	}

    // Update is called once per frame
    protected override void CalcPosition () {
        
        if (CameraActivated)
        {
            CameraOffset += cameraInput.control.vector3 * MovementSpeed;
            LookatOffset += lookAtInput.control.vector3 * LookSpeed;
        }
        else
        {
            OriginalPosition = CurrentPoint;
        }
        if (CameraActivated)
        {
            Vector3 AnimalOffset = currentAnimal.transform.position;
            AnimalOffset += CameraOffset;

            if (LookAtTransform != null)
                SetCurrentPoint(AnimalOffset);
        }
    }

    protected override void OnUpdate()
    {
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
    }

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
