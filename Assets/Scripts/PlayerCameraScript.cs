using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour {


    public GameObject pc_mainObject;
    private Transform pc_ObjFollowed;
    Matrix4x4 pc_B = new Matrix4x4();

    void Awake () {
        pc_ObjFollowed = pc_mainObject.transform;
        Base(pc_ObjFollowed);
    }
	
	void LateUpdate () {
        placeAt(pc_ObjFollowed,new Vector3(0.0f, 0.0f, -3.0f));
    }
    void Base(Transform newBase)
    {
        int pc_MatrixSize = 4;
        Vector4[] pc_Columnes = new Vector4[pc_MatrixSize];
        

        pc_Columnes[0] = new Vector4(newBase.right.x, newBase.right.y, newBase.right.z, 0.0f);
        pc_Columnes[1] = new Vector4(newBase.up.x, newBase.up.y, newBase.up.z, 0.0f);
        pc_Columnes[2] = new Vector4(newBase.forward.x, newBase.forward.y, newBase.forward.z, 0.0f);
        pc_Columnes[3] = new Vector4(newBase.position.x, newBase.position.y, newBase.position.z, 1.0f);

        for (int i = 0; i<pc_MatrixSize; i++) 
            pc_B.SetColumn(i, pc_Columnes[i]);

    }
    void ChangeBase (string pc_direction)
    {
        if (pc_direction.Equals("toWorld"))
            transform.position = pc_B.MultiplyPoint(transform.position);
        else if (pc_direction.Equals("toObject"))
            transform.position = pc_B.inverse.MultiplyPoint(transform.position);
    }
    void placeAt (Transform pc_reference, Vector3 pc_NewPosition)
    {
        Base(pc_reference);
        ChangeBase("toObject");
        transform.position = pc_NewPosition;
        ChangeBase("toWorld");
    }
}