using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{

    [Header("General")]

    [SerializeField] Transform target;                                                                      //Variable que apunta al objeto a seguir
    PlayerMovement playerMovementScript;                                                                    //Enlace al script de movimiento del jugador para poder acceder al desplazamiento
    [SerializeField] [Tooltip("Indica cuanto se acerca la camara al jugador al hacer colision de raycast en %")] float cameraScaleFactor = 90f;
    float originalCameraTargetPosition;                                                                     //Guardamos el valor original de la distancia jugador-camara en el caso de input para recolocarla al calcular la colision del raycast
    bool cameraMode = true;                                                                                 //Determina si usamos modo de camara automatica o con input manual

    [Header("Variables para camara automatica")]

    [SerializeField] float distanceFromObject = 5f;                                                         //Variable que indica la distancia inicial en el eje z
    [SerializeField] float cameraHeight = 2f;                                                               //Variable que almacena la posición constante de la cámara en el eje y
    float distanceMagnitude;                                                                                //Variable que guardará el el módulo de la distancia inicial
    [SerializeField] float maxRotationAngle = 110f;                                                         //Variable que indica el ángulo límite a partir del cual la cámara no gira
    [SerializeField] short delay = 8;                                                                       //Delay de la cámara, no debe ser inferior a 2
    Vector3[] delayPositions;

    [Header("Variables para camara de input manual")]

    [SerializeField] [Tooltip("Distancia entre camara y jugador \nNota: cada vez que se modifique su valor se debe reiniciar el juego")] float targetDistanceInputCamera = 10f;
    [SerializeField] [Tooltip("Sensibilidad en x")] float sensitivityX = 1f;                                 //Determina la sensibilidad de la camara de input manual en el eje x
    [SerializeField] [Tooltip("Sensibilidad en y")] float sensitivityY = 1f;                                 //Determina la sensibilidad de la camara de input manual en el eje y
    [SerializeField] [Tooltip("Angulo máxmimo que gira la camara en y")] float minYAngle = 0f;               //Determina el minimo angulo que la camara de input manual podra rotar
    [SerializeField] [Tooltip("Angulo mínimo que gira la camara en x")] float maxYAngle = 80f;                //Determina el maximo angulo que la camara de input manual podra rotar
    float xInput;                                                                                           //Guarda el input del eje x del stick para la camara de input manual
    float yInput;                                                                                           //Guarda el input del eje x del stick para la camara de input manual

    //FUNCIONES PRINCIPALES
    void Awake()
    {
        originalCameraTargetPosition = targetDistanceInputCamera;                                           //Guardamos la distancia original entre jugador y camara en el caso de input manual
        distanceMagnitude = new Vector3(0, cameraHeight, distanceFromObject).magnitude;                     //Calculo el módulo del vector de distancia máxima
        playerMovementScript = target.GetComponent<PlayerMovement>();                                       //Enlazamos con el script del personaje
        cameraScaleFactor /= 100f;                                                                          //Dividimos entre 100 el factor de escala, el codigo opera de 0 a 1 y en el inspector se muestra %
        delayPositions = new Vector3[delay];
        for (int i = 0; i < delay; i++)
        {
            delayPositions[i] = Vector3.zero;
        }
    }
    void Update()
    {
        xInput += Input.GetAxis("Mouse X") * sensitivityX;                                                  //Cogemos input en el eje x para la camara con input
        yInput += Input.GetAxis("Mouse Y") * sensitivityY;                                                  //Cogemos input en el eje x para la camara con input

        yInput = Mathf.Clamp(yInput, minYAngle, maxYAngle);                                                 //Limitamos el valor del input en y para que la camara no pueda girar completamente alrededor del jugador como hace en el eje x
    }

    void LateUpdate()
    {
        CheckCameraModeAndUpdate();                                                                         //Actualizo la posición de la cámara
    }

    //FUNCIONES AUXILIARES
    void UpdateAutomaticCamera(Transform obj)                                                               //Este método actualiza la posición de mi cámara respecto del objeto a seguir cuando la cámara no es libre
    {
        //Local vars
        Vector3 displacement = Displacement(obj);
        print(displacement);
        Vector3 objCamVec = transform.position - obj.position;
        float targetCameraAngle = StaticFunctions.AngleInPlane(transform.forward, obj.forward, new Vector3(0, 1, 0)); //Variable que guarda el ángulo que forman los dos vectores anteriores
        //Actions
        if (objCamVec.magnitude > distanceMagnitude)                                                          //Si el módulo de este vector es mayor que la distancia máxima de la cámara al personaje
            transform.position = obj.position + ((distanceMagnitude * objCamVec) / objCamVec.magnitude);        //Lo reescalamos al máximo permitido y recolocamos la cámara

        transform.position = new Vector3(transform.position.x, obj.position.y + cameraHeight, transform.position.z);//Asigno una constante altura

        if (targetCameraAngle <= maxRotationAngle)                                                           //Si mi ángulo no supera el máximo a partir del cual decido que la cámara no gire:
        {
            CheckObstacles(obj);                                                                            //Compruebo si hay obstáculos entre la cámara y el objetivo
            transform.LookAt(obj);                                                                          //Hago que la cámara mire al objeto a seguir
        }
        else
        {
            transform.position += displacement;      //La nueva posición será la misma de antes más el desplazamiento que haya realizado el objeto a seguir en el último frame
            CheckObstacles(obj);                                                                            //Compruebo si hay obstáculos entre la cámara y el objetivo
        }
    }
    void UpdateManualInputCamera(Transform obj)                                                             //Este método actualiza la posición de mi cámara respecto del objeto a seguir cuando la cámara es libre (controlada por el jugador)
    {
        Vector3 dir = new Vector3(0, 0, -targetDistanceInputCamera);                                        //Este vector nos sirve para mover la camara lejos del jugador segun la distancia establecida
        Quaternion rotation = Quaternion.Euler(yInput, xInput, 0);                                          // Guardamos la rotacion en un quaternion (Un quaternion tiene 4 variables, x y z determinan un vector y w determina cuanto giramos alrededor de ese vector) pero se lo pasamos en angulos de euler
        transform.position = target.position + rotation * dir;                                              //Llevamos la camara al jugador, aplicamos la rotacion y alejamos la camara segun la direccion establecida (un quaternion se puede operar como si fuera un vector)
<<<<<<< HEAD
        CheckObstacles(Obj);                                                                                //Comprobamos si algun objeto corta la linea de vision de la camara al jugador
=======
        CheckObstacles(obj);                                                                                //Comprobamos si algun objeto corta la linea de vision de la camara al jugador
        transform.position = target.position + rotation * dir;                                           //Llevamos la camara al jugador, aplicamos la rotacion y alejamos la camara segun la direccion establecida (un quaternion se puede operar como si fuera un vector)
        CheckObstacles(obj);                                                                                //Comprobamos si algun objeto corta la linea de vision de la camara al jugador
>>>>>>> 82c35ec696867ace4116e537178f33a8d487c46a
        transform.LookAt(target);                                                                           //Hacemos que la camara mire al jugador
    }
    void CheckObstacles(Transform obj)                                                                      //Este método comprueba si hay algún obstáculo entre la cámara y el objetivo, y además la recoloca.
    {
        //Local vars
        RaycastHit pc_hit;                                                                                  //Variable que almacenará datos sobre la colisión del raycast
        int pc_StaticSolid_mask = LayerMask.GetMask(StaticVariables.pc_StaticSolidLayer);                   //Esta layer_mask será utilizada para que el raycast solo tenga en cuenta la layer StaticSolid (mejor rendimiento que checkear cada objeto de la escena)
        float distanceCamTarget = (obj.position - transform.position).magnitude;                            //Calculamos la distancia entre la camara y el jugador
        Vector3 newDistanceCamTarget;                                                                       //Guardamos la posicion acercada de la camara si se corta la linea de vision

        //Actions
        if (Physics.Raycast(obj.position, -transform.TransformDirection(Vector3.forward), out pc_hit, distanceCamTarget, pc_StaticSolid_mask)) //Comprobamos la colision del rayo que va del jugador a la posicion de la camara
        {
            {//Lanzo un rayo desde el objetivo a la cámara, que solo colisionará con la layer StaticSolid y que tendrá la misma longitud que la distancia entre objetivo y cámara
                if (!cameraMode)
                {
                    newDistanceCamTarget = (pc_hit.point - obj.position) * cameraScaleFactor;               //Reescalamos el vector entre el punto de colision y el jugador para recolocar la camara
                    transform.position = obj.position + newDistanceCamTarget;                               //La nueva posición de la cámara será el primer punto de intersección detectado entre el objetivo y la cámara con la layer StaticSolid
                    targetDistanceInputCamera = (transform.position - obj.position).magnitude;              //Actualizamos la posicion de la camara en la variable que opera las rotaciones en el input manual
                }
                else
                {
                    transform.position = pc_hit.point;                                                      //Colocamos la camara en el punto de colision
                }

            }
        }
        else
        {
            targetDistanceInputCamera = originalCameraTargetPosition;                                      // Si no ha habido colision la camara vuelve a su posicion original
        }


        /*
         * Un problema de esto es que no detecta la colisión con un objeto si la cámara está dentro del mismo
         * Por lo cual, tiene más sentido hacer el raycast del objeto a la cámara
         * Por ellos lanzamos el rayo del objetivo a la cámara y no al revés
         */
    }
    void CheckCameraModeAndUpdate()                                                                         //Permite cambiar entre modo de camara automatico y manual pulsando la tecla Y
    {
        if (Input.GetKeyDown("y"))
        {
            cameraMode = !cameraMode;
        }
        if (cameraMode)
        {
            UpdateAutomaticCamera(target);
        }
        else
        {
            UpdateManualInputCamera(target);
        }
    }
    void UpdateDelay(Transform obj)
    {
        for (int i = 0; i < delay - 1; i++)
        {
            delayPositions[i] = delayPositions[i + 1];
        }
        delayPositions[delay - 1] = obj.position;
    }
    Vector3 Displacement(Transform obj)
    {
        UpdateDelay(obj);
        return (delayPositions[1] - delayPositions[0]);
    }
}

/*To do
    En el modo automático al ir el personaje hacia la cámara se nota un parpadeo
    En el modo automático, mientras la cámara esta pegada a una pared no puede rotar
 */
