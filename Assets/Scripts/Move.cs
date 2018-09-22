using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    // Use this for initialization

    Vector3 direction;
    public float aceleration = 0.015f;
    public float MaxSpeed = 0.5f;
    public GameObject Camera;
    Vector3 CameraLookAt;
    Vector3 CameraRight;
    public Vector3 Displacement;

    void Awake () {
        direction = Vector3.zero;

        
	}
	
	// Update is called once per frame
	void Update () {
        Displacement = transform.position;
        transform.Rotate(-90, 0, 0);
        CameraLookAt = Camera.transform.forward.normalized;
        CameraRight = Camera.transform.right.normalized;

        CameraLookAt = Vector3.ProjectOnPlane(CameraLookAt, new Vector3(0, 1, 0));
        CameraRight = Vector3.ProjectOnPlane(CameraRight, new Vector3(0, 1, 0));

        
        if (Input.GetKey("w"))
        {
            direction += CameraLookAt;
        }
        if (Input.GetKey("s"))
        {
            direction += -CameraLookAt;
        }
        if (Input.GetKey("d"))
        {
            direction += CameraRight;
        }
        if (Input.GetKey("a"))
        {
            direction += -CameraRight;
        }

        if (!Input.anyKey)
        {
            direction -= direction*aceleration*5;
            if (direction.magnitude < 0.05f)
            {
                direction = Vector3.zero;
            }
        }

        if(direction.magnitude > MaxSpeed)
        {
            direction = (direction * MaxSpeed) / direction.magnitude;
        }
        if(direction.magnitude>0)
            transform.forward = direction.normalized;

        transform.position += direction;
        Displacement = transform.position - Displacement;
    }
}
