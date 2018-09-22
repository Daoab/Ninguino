using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour {

    //DECLARACIÓN DE VARIABLES
    public GameObject pc_mainObject;    //Variable que apunta al objeto a seguir
    private Transform pc_ObjFollowed;   //Variable que guarda el transform del objeto a seguir
    public Move ObjMove;                //Variable que guarda el objeto de la clase Move del objeto a seguir

    Matrix4x4 pc_B = new Matrix4x4();   //Variable que guarda la matriz de cambio de base

    public float pc_distanceFromObjectInZ = 5f; //Variable que indica la distancia inicial en el eje z
    public float pc_CameraHeight = 2f;          //Variable que almacena la posición constante de la cámara en el eje y
    private float pc_realDistance;              //Variable que guardará el el módulo de la distancia inicial
    public float pc_MaxAngle = 90f;             //Variable que indica el ángulo límite a partir del cual la cámara no gira

    //public LayerMask StaticSolidLayer = -1;

    //FUNCIONES PRINCIPALES
    void Awake()
    {
        pc_ObjFollowed = pc_mainObject.transform;   //Guardo el transform del objeto a seguir en la variable pc_ObjFollowed
        pc_realDistance = new Vector3(0, pc_CameraHeight, pc_distanceFromObjectInZ).magnitude;  //Calculo el módulo del vector de distancia máxima
        ObjMove = pc_mainObject.GetComponent<Move>();   //Almaceno el objeto de la clase Move del objeot a seguir en una variable
    }

	void LateUpdate () {
        UpdateCameraPos(pc_ObjFollowed);            //Actualizo la posición de la cámara
        CheckObstacles(pc_ObjFollowed);
        pc_mainObject.transform.Rotate(90, 0, 0);   //Esto solo sirve para ayuda en el desarrollo, se puede quitar más adelante
    }

    //FUNCIONES AUXILIARES
    void Base(Transform newBase)            //Método que calcula la matriz de cambio de base para pasar la cámara a coordenadas del objeto a seguir
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
    void ChangeBase (string pc_direction)   //Método que cambia de base, tiene dos modos
    {
        if (pc_direction.Equals("toWorld"))         //en este modo devolvemos a la cámara a coordenadas del mundo
            transform.position = pc_B.MultiplyPoint(transform.position);
        else if (pc_direction.Equals("toObject"))   //en este modo llevamos a la cámara a coordenadas del objeto
            transform.position = pc_B.inverse.MultiplyPoint(transform.position);
    }
    void UpdateCameraPos(Transform Obj)     //Este método actualiza la posición de mi cámara respecto del objeto a seguir
    {
        //Base Change
        Base(pc_ObjFollowed);   //Calculo la matriz de cambio de base
        ChangeBase("toObject"); //Paso a coordenadas del objeto a seguir
        //Local vars
        Vector3 pc_NewPos = Vector3.zero;   //Creo un vector que almacenará la nueva posición en coordenadas del objeto
        Vector3 pc_ObjVector = Vector3.ProjectOnPlane(Obj.forward, new Vector3(0, 1, 0)).normalized;         //Variable que guarda la proyección normalizada del vector forward(z) del objeto
        Vector3 pc_CameraVector = Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0)).normalized;//Variable que guarda la proyección normalizada del vector forward(z) de la cámara
        float pc_Angle = Vector3.Angle(pc_CameraVector, pc_ObjVector);  //Variable que guarda el ángulo que forman los dos vectores anteriores
        //Actions
        if (pc_Angle < pc_MaxAngle) //Si mi ángulo no supera el máximo a partir del cual decido que la cámara no gire:
        {
            pc_NewPos = transform.position; //Al estar en coordenadas del objeto, transform.position es el vector que une el objeto a seguir con la cámara
            if (pc_NewPos.magnitude > pc_realDistance)  //Si el módulo de este vector es mayor que la distancia máxima de la cámara al personaje
            {
                pc_NewPos = (pc_realDistance * pc_NewPos) / pc_NewPos.magnitude;//Lo reescalamos al máximo permitido
            }
            transform.position = pc_NewPos; //La posición será este vector reescalado que une ambos objetos
            ChangeBase("toWorld");  //Vuelvo a coordenadas del mundo
            transform.position = new Vector3(transform.position.x, pc_CameraHeight, transform.position.z);  //Asigno una constante altura
            transform.LookAt(pc_ObjFollowed);   //Hago que la cámara mire al objeto a seguir
        }
        else
        {
            ChangeBase("toWorld"); //Vuelvo a coordenadas del mundo
            pc_NewPos = transform.position + ObjMove.Displacement;  //La nueva posición será la misma de antes más el desplazamiento que haya realizado el objeto a seguir en el último frame
            transform.position = pc_NewPos; //Asigno esta nueva posición
            transform.position = new Vector3(transform.position.x, pc_CameraHeight, transform.position.z);  //Asigno una constante altura
        }
    }
    void CheckObstacles(Transform Obj)      //Este método comprueba si hay algún obstáculo entre la cámara y el objetivo, y además la recoloca.
    {
        RaycastHit hit;
        int layer_mask = LayerMask.GetMask(StaticVariables.pc_StaticSolidLayer);

        if(Physics.Raycast(Obj.position,-transform.TransformDirection(Vector3.forward),out hit, (Obj.position - transform.position).magnitude,layer_mask))
        {
            print(hit.distance + " " + hit.collider.gameObject.name);
        }
        /*
         * Un problema de esto es que no detecta la colisión con un objeto si la cámara está dentro del mismo
         * Por lo cual, tiene más sentido hacer el raycast del objeto a la cámara
         */
    }
}