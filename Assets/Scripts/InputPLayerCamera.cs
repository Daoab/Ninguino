using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPLayerCamera : MonoBehaviour {

    [SerializeField] Transform MJ_target;
    [SerializeField] float MJ_targetDistance = 10f;

    [SerializeField] float MJ_sensitivityX = 1f;
    [SerializeField] float MJ_sensitivityY = 1f;

    [SerializeField] float MJ_yMin = 0f;
    [SerializeField] float MJ_yMax = 80f;


    float MJ_inputX;
    float MJ_inputY;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        MJ_inputX += Input.GetAxis("Mouse X") * MJ_sensitivityX;
        MJ_inputY += Input.GetAxis("Mouse Y") * MJ_sensitivityY;

        MJ_inputY = Mathf.Clamp(MJ_inputY, MJ_yMin, MJ_yMax);

	}

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -MJ_targetDistance);
        Quaternion MJ_rotation = Quaternion.Euler(MJ_inputY, MJ_inputX, 0);
        transform.position = MJ_target.position + MJ_rotation * dir;
        transform.LookAt(MJ_target);
    }
}
