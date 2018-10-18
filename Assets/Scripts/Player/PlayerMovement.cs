using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Tooltip("Velocidad de movimiento del personaje")] public float movementSpeed = 10f;       // Velocidad de movimiento del personaje
    [SerializeField] [Tooltip("Enlace a la camara del jugador")] Transform playerCamera;                 // Enlace a la camara que seguirá al jugador para basar el movimiento en sus ejes


    public Rigidbody rb;

    float xThrow;                                                                                       //Cantidad que mueves el stick en su eje x
    float yThrow;                                                                                       //Cantidad que mueves el stick en su eje y

    Vector3 cameraForward;                                                                              //Aqui guardamos el vector forward (z) de la camara del jugador
    Vector3 cameraRight;                                                                                // Aqui guardamos el vector right (x) de la camara del jugador

    Vector3 movement;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();                                                      // Guardamos el componente rigidbody del jugador
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
    void MovePlayer()
    {
        xThrow = Input.GetAxis("Horizontal");                                                           //Asignamos el valor del eje x a su variable
        yThrow = Input.GetAxis("Vertical");                                                             //Asignamos el valor del eje y a su variable

        cameraForward = Vector3.ProjectOnPlane(playerCamera.forward, new Vector3(0, 1, 0)).normalized;    //Guardamos el vector forward de la camara
        cameraRight = Vector3.ProjectOnPlane(playerCamera.right, new Vector3(0, 1, 0)).normalized;    //Guardamos el vector right de la camara

        movement = (cameraRight * xThrow + cameraForward * yThrow) * movementSpeed;                     //Multiplicamos los vectores por el throw para ver la direccion de movimiento y por movementSpeed para ver su intensidad

        rb.velocity = movement;
        transform.LookAt(transform.position + movement);                                        //El jugador mira hacia donde avanza
    }
}