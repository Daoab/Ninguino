using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{

    [Header("General")]

    [SerializeField] [Tooltip("Distancia entre camara y jugador \nNota: cada vez que se modifique su valor se debe reiniciar el juego")] float targetCamDistance = 8f;
    [SerializeField] [Tooltip("Indica cuanto se acerca la camara al jugador al hacer colision de raycast en %")] float cameraScaleFactor = 90f;
    [SerializeField] Transform target;                                                                          //Variable que apunta al objeto a seguir     
    [SerializeField] float distanceWhenRaycast = 0.11f;                                                         //Para que se vea la pared, debe ser mayor que el plano near
    float originalCameraTargetPosition;                                                                         //Guardamos el valor original de la distancia jugador-camara en el caso de input para recolocarla al calcular la colision del raycast
    bool automaticMode = true;                                                                                  //Determina si usamos modo de camara automatica o con input manual


    [Header("Variables para camara automatica")]

    [SerializeField] float cameraHeight = 2f;                                                                   //Variable que almacena la posición constante de la cámara en el eje y
    [SerializeField] float maxRotationAngle = 110f;                                                             //Variable que indica el ángulo límite a partir del cual la cámara no gira
    [SerializeField] short delay = 8;                                                                           //Delay de la cámara, no debe ser inferior a 2
    Transform[] delayTransform;
    bool LookingAtCamera = false;
    Vector3 vecDisplacement;


    [Header("Variables para camara de input manual")]

    [SerializeField] [Tooltip("Sensibilidad en x")] float sensitivityX = 1f;                                    //Determina la sensibilidad de la camara de input manual en el eje x
    [SerializeField] [Tooltip("Sensibilidad en y")] float sensitivityY = 1f;                                    //Determina la sensibilidad de la camara de input manual en el eje y
    [SerializeField] [Tooltip("Angulo máxmimo que gira la camara en y")] float minYAngle = 0f;                  //Determina el minimo angulo que la camara de input manual podra rotar
    [SerializeField] [Tooltip("Angulo mínimo que gira la camara en x")] float maxYAngle = 80f;                  //Determina el maximo angulo que la camara de input manual podra rotar
    float xInput, yInput;                                                                                       //Guarda el input del eje x y del eje y del stick para la camara de input manual


    //FUNCIONES PRINCIPALES
    void Start()
    {
        originalCameraTargetPosition = targetCamDistance;                                                       //Guardamos la distancia original entre jugador y camara en el caso de input manual
        cameraScaleFactor /= 100f;                                                                              //Dividimos entre 100 el factor de escala, el codigo opera de 0 a 1 y en el inspector se muestra %
        InitializeDelay();
    }
    void Update()
    {
        xInput += Input.GetAxis("Mouse X") * sensitivityX;                                                      //Cogemos input en el eje x para la camara con input
        yInput = Mathf.Clamp(yInput + Input.GetAxis("Mouse Y") * sensitivityY, minYAngle, maxYAngle);           //Cogemos input en el eje y y lo limitamos el valor del input en y para que la camara no pueda girar completamente alrededor del jugador como hace en el eje x
    }

    void LateUpdate()
    {
        CheckCameraModeAndUpdate();                                                                             //Actualizo la posición de la cámara
    }
    //FUNCIONES AUXILIARES DE START
    void InitializeDelay()
    {
        delayTransform = new Transform[delay];
        for (int i = 0; i < delay; i++)
            delayTransform[i] = target.transform;
    }
    //FUNCIONES AUXILIARES DE UPDATE
    void UpdateDelay()
    {
        for (int i = 0; i < delay - 1; i++)
            delayTransform[i] = delayTransform[i + 1];
        delayTransform[delay - 1] = target;
    }
    void CheckCameraModeAndUpdate()                                                                             //Permite cambiar entre modo de camara automatico y manual pulsando la tecla Y
    {
        UpdateDelay();
        if (Input.GetKeyDown("y"))
            automaticMode = !automaticMode;
        if (automaticMode)
            UpdateAutomaticCamera(delayTransform[0]);
        else
            UpdateManualInputCamera(delayTransform[0]);
    }
    void UpdateAutomaticCamera(Transform obj)                                                                   //Este método actualiza la posición de mi cámara respecto del objeto a seguir cuando la cámara no es libre
    {
        //Local vars
        Vector3 objToCamVec = transform.position - obj.position;
        float actualAngle = StaticFunctions.AngleInPlane(transform.forward, obj.forward, Vector3.up);           //Variable que guarda el ángulo que forman los dos vectores anteriores

        if (actualAngle <= maxRotationAngle)
        {                                                                                                       //Si mi ángulo no supera el máximo a partir del cual decido que la cámara no gire:
            LookingAtCamera = false;
            if (objToCamVec.magnitude > targetCamDistance)                                                          //Si el módulo de este vector es mayor que la distancia máxima de la cámara al personaje
                transform.position = obj.position + ((targetCamDistance * objToCamVec) / objToCamVec.magnitude);    //Lo reescalamos al máximo permitido y recolocamos la cámara
        }
        else
        {
            if (!LookingAtCamera)
            {
                vecDisplacement = transform.position - obj.position;
                LookingAtCamera = true;
            }
            else
                transform.position = obj.position + vecDisplacement;
        }
        CheckObstacles();
        transform.LookAt(obj);

    }
    void UpdateManualInputCamera(Transform obj)                                                                 //Este método actualiza la posición de mi cámara respecto del objeto a seguir cuando la cámara es libre (controlada por el jugador)
    {
        Vector3 dir = new Vector3(0, 0, -targetCamDistance);                                                    //Este vector nos sirve para mover la camara lejos del jugador segun la distancia establecida
        Quaternion rotation = Quaternion.Euler(yInput, xInput, 0);                                              // Guardamos la rotacion en un quaternion (Un quaternion tiene 4 variables, x y z determinan un vector y w determina cuanto giramos alrededor de ese vector) pero se lo pasamos en angulos de euler
        transform.position = target.position + rotation * dir;                                                  //Llevamos la camara al jugador, aplicamos la rotacion y alejamos la camara segun la direccion establecida (un quaternion se puede operar como si fuera un vector)
        CheckObstacles();
        transform.LookAt(target);                                                                               //Hacemos que la camara mire al jugador
    }

    void CheckObstacles()
    {
        int staticSolid_mask = LayerMask.GetMask(StaticVariables.pc_StaticSolidLayer);
        Vector3 objToCamVec = transform.position - target.position;
        float heightDifference;
        //Actions
        transform.position = StaticFunctions.CheckObstaclesAndBringCloser(target.position, objToCamVec, objToCamVec.magnitude, staticSolid_mask, distanceWhenRaycast, transform.position, transform.position);
        if (automaticMode)
        {
            heightDifference = (target.position.y + cameraHeight) - transform.position.y;

            transform.position = StaticFunctions.CheckObstaclesAndBringCloser(transform.position, Vector3.up, heightDifference, staticSolid_mask,
                distanceWhenRaycast, transform.position, new Vector3(transform.position.x, target.position.y + cameraHeight, transform.position.z));
        }
    }
}