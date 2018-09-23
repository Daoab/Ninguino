using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] float MJ_movementSpeed = 10f;
    [SerializeField] Transform MJ_playerCamera;

    float MJ_xThrow;
    float MJ_yThrow;

    Vector3 MJ_cameraForward;
    Vector3 MJ_cameraRight;

    [HideInInspector] public Vector3 displacement;
	
	// Update is called once per frame
	void Update () {
        displacement = transform.position;
        MJ_xThrow = Input.GetAxis("Horizontal");
        MJ_yThrow = Input.GetAxis("Vertical");

        MJ_cameraForward = MJ_playerCamera.forward;
        MJ_cameraRight = MJ_playerCamera.right;

        MJ_cameraForward.y = 0f;
        MJ_cameraRight.y = 0f;

        MJ_cameraRight = MJ_cameraRight.normalized;
        MJ_cameraForward = MJ_cameraForward.normalized;

        transform.position += (MJ_cameraRight * MJ_xThrow + MJ_cameraForward * MJ_yThrow) * MJ_movementSpeed * Time.deltaTime;
        displacement = transform.position - displacement;
        transform.LookAt(transform.position + displacement);
	}
}
