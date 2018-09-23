using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour {

    //DECLARACIÓN DE VARIABLES
    [SerializeField] Transform target;    //Variable que apunta al objeto a seguir
    PlayerMovement playerMovementScript;

    

    Matrix4x4 pc_B = new Matrix4x4();   //Variable que guarda la matriz de cambio de base

    [SerializeField] float pc_distanceFromObjectInZ = 5f; //Variable que indica la distancia inicial en el eje z
    public float pc_CameraHeight = 2f;          //Variable que almacena la posición constante de la cámara en el eje y
    private float pc_realDistance;              //Variable que guardará el el módulo de la distancia inicial
    public float pc_MaxAngle = 90f;             //Variable que indica el ángulo límite a partir del cual la cámara no gira

    [SerializeField] bool pc_CameraMode = false;
    [SerializeField] float cameraScaleFactor = 50f;



    //VARIABLES CAMERA INPUT

    [SerializeField] float MJ_targetDistance = 10f;

    [SerializeField] float MJ_sensitivityX = 1f;
    [SerializeField] float MJ_sensitivityY = 1f;

    [SerializeField] float MJ_yMin = 0f;
    [SerializeField] float MJ_yMax = 80f;


    float MJ_inputX;
    float MJ_inputY;

    //FUNCIONES PRINCIPALES
    void Awake()
    {
        pc_realDistance = new Vector3(0, pc_CameraHeight, pc_distanceFromObjectInZ).magnitude;  //Calculo el módulo del vector de distancia máxima
        playerMovementScript = target.GetComponent<PlayerMovement>();
        cameraScaleFactor /= 100f;
    }
    void Update()
    {
        MJ_inputX += Input.GetAxis("Mouse X") * MJ_sensitivityX;
        MJ_inputY += Input.GetAxis("Mouse Y") * MJ_sensitivityY;

        MJ_inputY = Mathf.Clamp(MJ_inputY, MJ_yMin, MJ_yMax);
    }

    void LateUpdate () {
        CheckCameraModeAndUpdate();            //Actualizo la posición de la cámara
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
    void AutoUpdateCamera(Transform Obj)     //Este método actualiza la posición de mi cámara respecto del objeto a seguir cuando la cámara no es libre
    {
        //Base Change
        Base(target);   //Calculo la matriz de cambio de base
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
            transform.position = pc_NewPos;     //La posición será este vector reescalado que une ambos objetos
            ChangeBase("toWorld");  //Vuelvo a coordenadas del mundo
            transform.position = new Vector3(transform.position.x, pc_CameraHeight, transform.position.z);  //Asigno una constante altura
            CheckObstacles(Obj);    //Compruebo si hay obstáculos entre la cámara y el objetivo
            transform.LookAt(target);   //Hago que la cámara mire al objeto a seguir
        }
        else
        {
            pc_NewPos = transform.position; //Al estar en coordenadas del objeto, transform.position es el vector que une el objeto a seguir con la cámara
            if (pc_NewPos.magnitude > pc_realDistance)  //Si el módulo de este vector es mayor que la distancia máxima de la cámara al personaje
            {
                pc_NewPos = (pc_realDistance * pc_NewPos) / pc_NewPos.magnitude;//Lo reescalamos al máximo permitido
            }
            transform.position = pc_NewPos;     //La posición será este vector reescalado que une ambos objetos
            ChangeBase("toWorld"); //Vuelvo a coordenadas del mundo
            pc_NewPos = transform.position + playerMovementScript.displacement;  //La nueva posición será la misma de antes más el desplazamiento que haya realizado el objeto a seguir en el último frame
            transform.position = pc_NewPos; //Asigno esta nueva posición
            transform.position = new Vector3(transform.position.x, pc_CameraHeight, transform.position.z);  //Asigno una constante altura
            CheckObstacles(Obj);    //Compruebo si hay obstáculos entre la cámara y el objetivo
        }
    }
    void InputUpdateCamera(Transform Obj)              //Este método actualiza la posición de mi cámara respecto del objeto a seguir cuando la cámara es libre (controlada por el jugador)
    {
        Vector3 dir = new Vector3(0, 0, -MJ_targetDistance);
        Quaternion MJ_rotation = Quaternion.Euler(MJ_inputY, MJ_inputX, 0);
        transform.position = target.position + MJ_rotation * dir;
        CheckObstacles(Obj);
        transform.LookAt(target);
    }
    void CheckObstacles(Transform Obj)      //Este método comprueba si hay algún obstáculo entre la cámara y el objetivo, y además la recoloca.
    {
        //Local vars
        RaycastHit pc_hit;  //Variable que almacenará datos sobre la colisión del raycast
        int pc_StaticSolid_mask = LayerMask.GetMask(StaticVariables.pc_StaticSolidLayer);   //Esta layer_mask será utilizada para que el raycast solo tenga en cuenta la layer StaticSolid
        float distanceCamTarget = (Obj.position - transform.position).magnitude;
        Vector3 newDistanceCamTarget;

        //Actions
        if(Physics.Raycast(Obj.position,-transform.TransformDirection(Vector3.forward),out pc_hit, distanceCamTarget, pc_StaticSolid_mask))
        {//Lanzo un rayo desde el objetivo a la cámara, que solo colisionará con la layer StaticSolid y que tendrá la misma longitud que la distancia entre objetivo y cámara
            if (!pc_CameraMode) {
                newDistanceCamTarget = (pc_hit.point - Obj.position) * cameraScaleFactor;
                transform.position = Obj.position + newDistanceCamTarget;                              //La nueva posición de la cámara será el primer punto de intersección detectado entre el objetivo y la cámara con la layer StaticSolid
                MJ_targetDistance = (transform.position - Obj.position).magnitude;
            }
            else
            {
                transform.position = pc_hit.point;
            }
        }



        /*
         * Un problema de esto es que no detecta la colisión con un objeto si la cámara está dentro del mismo
         * Por lo cual, tiene más sentido hacer el raycast del objeto a la cámara
         * Por ellos lanzamos el rayo del objetivo a la cámara y no al revés
         */
    }
    void CheckCameraModeAndUpdate()
    {
        if (Input.GetKeyDown("y"))
        {
            pc_CameraMode = !pc_CameraMode;
        }
        if (pc_CameraMode)
        {
            AutoUpdateCamera(target);
        }
        else
        {
            InputUpdateCamera(target);
        }
    }
}

/*To do
 * Arreglar las colisiones para que no haga tururr turrur
 * Arreglar el giro en el modo de cámara automátic@
 * Recalcular distancias cuando hay colisión
 * Arreglar el raycast para el caso de la cámara automática
 * Poner las variables en el inspector bonitas
 * Dejar el código bonito
 */