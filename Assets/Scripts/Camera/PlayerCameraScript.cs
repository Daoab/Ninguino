using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour {

    public GameObject pc_mainObject;    //Variable que apunta al objeto a seguir
    private Transform pc_ObjFollowed;   //Variable que guarda el transform del objeto a seguir
    Matrix4x4 pc_B = new Matrix4x4();   //Variable que guarda la matriz de cambio de base
    public float pc_distanceFromObject;
    public float pc_CameraHeight;
    Vector3 pc_NewPos = new Vector3(0, 0, 0);
    Vector3 pc_LastPos = new Vector3(0, 0, 0);
    public float pc_MaxAngle = 110f;

    void Awake () {
        pc_ObjFollowed = pc_mainObject.transform;   //Guardo el transform del objeto a seguir en la variable pc_ObjFollowed
        pc_distanceFromObject = 3.5f;
        pc_CameraHeight = 0.5f;
    }
	


	void LateUpdate () {
        Base(pc_ObjFollowed);
        ChangeBase("toObject");
        //pc_NewPos = transform.position;
        placeAt(NewCameraPos(pc_ObjFollowed));
        transform.LookAt(pc_ObjFollowed);
        pc_mainObject.transform.Rotate(90, 0, 0);
    }
    void Base(Transform newBase)    //Método que calcula la matriz de cambio de base para pasar la cámara a coordenadas del objeto a seguir
    {
        int pc_MatrixSize = 4;  //Defino el tamaño de la matriz, su única utilidad es poner límite a los bucles for
        Vector4[] pc_Columnes = new Vector4[pc_MatrixSize]; //Defino un array de Vector4 que guardará las columnas de la matriz de cambio de base
        
        //Doy valor a las columnas según la fórmula para calcular la matriz de cambio de base
        pc_Columnes[0] = new Vector4(newBase.right.x, newBase.right.y, newBase.right.z, 0.0f);
        pc_Columnes[1] = new Vector4(newBase.up.x, newBase.up.y, newBase.up.z, 0.0f);
        pc_Columnes[2] = new Vector4(newBase.forward.x, newBase.forward.y, newBase.forward.z, 0.0f);
        pc_Columnes[3] = new Vector4(newBase.position.x, newBase.position.y, newBase.position.z, 1.0f);

        for (int i = 0; i<pc_MatrixSize; i++) //En este bucle seteo cada columna de la matriz a su columna correspondiente del array
            pc_B.SetColumn(i, pc_Columnes[i]);

    }
    void ChangeBase (string pc_direction) //Método que cambia de base, tiene dos modos
    {
        if (pc_direction.Equals("toWorld"))         //en este modo devolvemos a la cámara a coordenadas del mundo
            transform.position = pc_B.MultiplyPoint(transform.position);
        else if (pc_direction.Equals("toObject"))   //en este modo llevamos a la cámara a coordenadas del objeto
            transform.position = pc_B.inverse.MultiplyPoint(transform.position);
    }
    void placeAt (Vector3 pc_NewPosition)   //Método que me permite colocar un objeto en una nueva posición respecto a la base de otro objeto
    {
        //Base(pc_reference);
        //ChangeBase("toObject");
        transform.position = pc_NewPosition;
        ChangeBase("toWorld");
    }
    Vector3 NewCameraPos(Transform Obj)
    {
        /*/Vector3 pc_NewPos = new Vector3(0, 0, 0);
        Vector3 ObjDirection = Obj.forward.normalized;

        pc_NewPos.x = ObjDirection.x;
        pc_NewPos.y = pc_CameraHeight;
        pc_NewPos.z = -ObjDirection.z;

        pc_NewPos *= pc_distanceFromObject;/**/

        /*/Vector3 pc_NewPos = transform.position;
        pc_NewPos = (pc_distanceFromObject * pc_NewPos) / pc_NewPos.magnitude;/**/
        Vector3 pc_ObjVector = Vector3.ProjectOnPlane(Obj.forward,new Vector3(0, 1, 0));
        Vector3 pc_CameraVector = Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0));
        float pc_angle = Vector3.Angle(pc_CameraVector, pc_ObjVector);
        if (pc_angle > pc_MaxAngle) //La cámara no debe girar
        {
            pc_NewPos = pc_LastPos;
        } else //La cámara debe girar
        {
            pc_NewPos = transform.position;
            pc_NewPos = (pc_distanceFromObject * pc_NewPos) / pc_NewPos.magnitude;
            pc_LastPos = pc_NewPos;
        }
        if(!Input.anyKey && pc_angle<10)
        {
            pc_NewPos = new Vector3(0, pc_CameraHeight, pc_distanceFromObject);
        }
        print(pc_angle);

        return pc_NewPos;
    }
}