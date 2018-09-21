using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour {

    public GameObject pc_mainObject;    //Variable que apunta al objeto a seguir
    private Transform pc_ObjFollowed;   //Variable que guarda el transform del objeto a seguir
    Matrix4x4 pc_B = new Matrix4x4();   //Variable que guarda la matriz de cambio de base

    void Awake () {
        pc_ObjFollowed = pc_mainObject.transform;   //Guardo el transform del objeto a seguir en la variable pc_ObjFollowed
    }
	
	void LateUpdate () {
        placeAt(pc_ObjFollowed,new Vector3(0.0f, 0.0f, -3.0f)); //Coloco a la cámara a una distancia cierta respecto del objeto a seguir
        transform.LookAt(pc_ObjFollowed);
    }
    void Base(Transform newBase)    //Método que calcula la matriz de cambio de base para pasar la cámara a coordenadas del objeto a seguir
    {
        int pc_MatrixSize = 4;  //Defino el tamaño de la matriz, su única utilidad es poner límite a los bucles for
        Vector4[] pc_Columnes = new Vector4[pc_MatrixSize]; //Defino un array de Vector4 que guardará las columnas de la matriz de cambio de base
        
        //Doy valor a las columnas en base a la fórmula para calcular la matriz de cambio de base
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
    void placeAt (Transform pc_reference, Vector3 pc_NewPosition)   //Método que me permite colocar un objeto en una nueva posición respecto a la base de otro objeto
    {
        Base(pc_reference);
        ChangeBase("toObject");
        transform.position = pc_NewPosition;
        ChangeBase("toWorld");
    }
}