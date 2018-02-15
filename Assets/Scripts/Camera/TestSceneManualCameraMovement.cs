using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class TestSceneManualCameraMovement : MonoBehaviour {

    private Vector3 OriginalPosition;
    private Vector3 CameraOffset;
    public Vector3Action cameraInput;
    public float Speed;
    public float lerpSpeed;
	// Use this for initialization
	void Start () {
        OriginalPosition = transform.position;
        cameraInput.Bind(GameManager.Instance.GetComponent<PlayerInput>().handle);
	}
	
	// Update is called once per frame
	void Update () {
        CameraOffset += cameraInput.control.vector3 * Speed;
        transform.position = Vector3.Lerp(transform.position, OriginalPosition + CameraOffset, Time.deltaTime * lerpSpeed);
    }
}
