using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class ManualCameraMovement : MonoBehaviour {

    private Vector3 OriginalPosition;
    private Vector3 CameraOffset;
    private Animal currentAnimal;
    private CameraSplineManager CameraSM;
    private bool CameraActivated = true;
    private bool doneToggle;
    private bool doneSwitch;

    public Vector3Action cameraInput;
    public ButtonAction CameraToggle1;
    public ButtonAction CameraToggle2;
    public Vector3 OriginOffset;
    public float Speed;
    public float lerpSpeed;
	// Use this for initialization
	void Start () {
        OriginalPosition = transform.position + OriginOffset;
        cameraInput.Bind(GameManager.Instance.input.handle);
        CameraToggle1.Bind(GameManager.Instance.input.handle);
        CameraToggle2.Bind(GameManager.Instance.input.handle);
        CameraSM = Camera.main.GetComponent<CameraSplineManager>();
	}
	
	// Update is called once per frame
	void Update () {
        currentAnimal = GameManager.Instance.GetComponent<AnimalController>().GetCurrentAnimal<Animal>();
        if (CameraActivated)
        {
            CameraOffset += cameraInput.control.vector3 * Speed;
        }
        if (currentAnimal != null)
        {
            Vector3 AnimalOffset = currentAnimal.transform.position;
            AnimalOffset += CameraOffset;
            transform.position = Vector3.Lerp(transform.position, OriginalPosition + AnimalOffset, Time.deltaTime * lerpSpeed);
        }
    }
}
