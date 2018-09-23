using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField][Tooltip("Velocidad de movimiento del personaje")] float movementSpeed = 10f;// Velocidad de movimiento del personaje
    [SerializeField][Tooltip("Enlace a la camara del jugador")] Transform playerCamera;          // Enlace a la camara que seguirá al jugador para basar el movimiento en sus ejes


    Rigidbody rb;

    float xThrow;                                                                                //Cantidad que mueves el stick en su eje x
    float yThrow;                                                                                //Cantidad que mueves el stick en su eje y

    Vector3 cameraForward;                                                                       //Aqui guardamos el vector forward (z) de la camara del jugador
    Vector3 cameraRight;                                                                         // Aqui guardamos el vector right (x) de la camara del jugador

    [HideInInspector] public Vector3 displacement;                                               //Se usa en el modo de camara automatica para el correcto seguimiento a la hora de correr hacia la camara

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();                                               // Guardamos el componente rigidbody del jugador
    }

    // Update is called once per frame
    void Update () {
        rb.velocity = Vector3.zero;                                                              //Eliminamos la velocidad que le quede al jugador cuando deje de dar input (evita movimiento resbaladizo)

        displacement = transform.position;                                                       // Guardamos posicion de este frame

        xThrow = Input.GetAxis("Horizontal");                                                    //Asignamos el valor del eje x a su variable
        yThrow = Input.GetAxis("Vertical");                                                      //Asignamos el valor del eje y a su variable

        cameraForward = playerCamera.forward;                                                    //Guardamos el vector forward de la camara
        cameraRight = playerCamera.right;                                                        //Guardamos el vector right de la camara

        cameraForward.y = 0f;                                                                    //Negamos la componente en y para que el jugador solo pueda andar en el plano xz
        cameraRight.y = 0f;                                                                      //Negamos la componente en y para que el jugador solo pueda andar en el plano xz

        cameraRight = cameraRight.normalized;                                                    //Normalizamos para actualizar su modulo a 1
        cameraForward = cameraForward.normalized;                                                //Normalizamos para actualizar su modulo a 1

        Vector3 movement = (cameraRight * xThrow + cameraForward * yThrow) * movementSpeed;      //Multiplicamos los vectores por el throw para ver la direccion de movimiento y por movementSpeed para ver su intensidad

        rb.AddForce(movement, ForceMode.VelocityChange);                                         //Aplicamos el vector movement al RigidBody en modo VelocityChange(ingnora su masa y es menos resbaladizo)

        //Antigua forma de moverse no basada en fisicas
        //transform.position += (cameraRight * xThrow + cameraForward * yThrow) * movementSpeed * Time.deltaTime; //Actualiazmos la posicion del jugador segun los valores de los vectores de la camara y la velocidad


        displacement = transform.position - displacement;                                        //Calculamos el desplazamiento del jugador entre frames
        transform.LookAt(transform.position + displacement);                                     //Hacemos que el jugador mire siempre a la dirección en la que avanza
	}
}
