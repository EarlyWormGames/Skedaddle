using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class TestSceneManualCameraMovement : MonoBehaviour {

    private Vector3 OriginalPosition;
    private Vector3 CameraOffset;
    private Animal currentAnimal;
    public Vector3Action cameraInput;
    public float Speed;
    public float lerpSpeed;
	// Use this for initialization
	void Start () {
        OriginalPosition = transform.position;
        cameraInput.Bind(GameManager.Instance.input.handle);
	}
	
	// Update is called once per frame
	void Update () {
        currentAnimal = GameManager.Instance.GetComponent<AnimalController>().GetCurrentAnimal();
        Vector3 AnimalOffset = currentAnimal.transform.position;
        CameraOffset += cameraInput.control.vector3 * Speed;
        AnimalOffset += CameraOffset;
        transform.position = Vector3.Lerp(transform.position, OriginalPosition + AnimalOffset, Time.deltaTime * lerpSpeed);
    }
}
