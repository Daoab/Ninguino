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
    public float LoseFactor = 8f;

    void Awake () {
        direction = Vector3.zero;

        
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(-90, 0, 0);
        CameraLookAt = Camera.transform.forward.normalized;
        CameraRight = Camera.transform.right.normalized;

        CameraLookAt = Vector3.ProjectOnPlane(CameraLookAt, new Vector3(0, 1, 0));
        CameraRight = Vector3.ProjectOnPlane(CameraRight, new Vector3(0, 1, 0));

        if (Input.GetKey("w"))
        {
            direction += CameraLookAt*aceleration;
        } else 
        {
            direction -= CameraLookAt * aceleration*LoseFactor;
        }
        if (Input.GetKey("s"))
        {
            direction -= CameraLookAt*aceleration;
        } else
        {
            direction += CameraLookAt * aceleration*LoseFactor;
        }
        if (Input.GetKey("d"))
        {
            direction += CameraRight * aceleration;
        } else
        {
            direction -= CameraRight * aceleration*LoseFactor;
        }
        if (Input.GetKey("a"))
        {
            direction -= CameraRight * aceleration;
        } else
        {
            direction += CameraRight * aceleration*LoseFactor;
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
        //print(direction);
    }
}
