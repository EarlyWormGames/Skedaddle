using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

[RequireComponent(typeof(Animal))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class AnimalMovement : MonoBehaviour
{
    public float BaseMoveSpeed = 10;
    public float MaxMoveSpeed = 20;
    public float DecelerationRate = 1;

    private MainMapping input;
    private float moveVelocity;
    private Rigidbody rig;

    // Use this for initialization
    void Start()
    {
        input = GetComponent<PlayerInput>().GetActions<MainMapping>();
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveVelocity += input.moveX.value * Time.deltaTime * BaseMoveSpeed;
        moveVelocity = Mathf.Clamp(moveVelocity, -MaxMoveSpeed, MaxMoveSpeed);
        moveVelocity = Mathf.MoveTowards(moveVelocity, 0, DecelerationRate * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rig.MovePosition(transform.position + (transform.forward * moveVelocity));
    }
}