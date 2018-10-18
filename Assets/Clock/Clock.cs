using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour {

    [SerializeField] Transform hoursArm, minutesArm, secondsArm;
    [SerializeField] bool smoothRotation = true;
    Quaternion hRotation, mRotation, sRotation;
    const float degreesPerHour = 30f;   //Cada indicar de hora del reloj esta rotado 30 grados, por eso *30
    const float degreesPerMinute = 6f;  //Hay 5 indicadores de segundo entre cada indicador de hora (30/6 = 5)
    const float degreesPerSecond = 6f;  //Hay 5 indicadores de minuto entre cada indicador de hora (30/6 = 5)

    DateTime timeDiscrete;           
    TimeSpan timeAnalog;   

	
	// Update is called once per frame
	void Update () {
        timeDiscrete = DateTime.Now;            //Nos da la hora de forma fractal, el reloj se actualiza automaticamente y da saltos (mejor para un reloj digital)
        timeAnalog = DateTime.Now.TimeOfDay;    //Devuelve un diferencial de tiempo, el reloj se mueve de manera suave

        if (smoothRotation)
        {
            UpdateAnalog();
        }
        else
        {
            UpdateDiscrete();
        }
    }

    private void UpdateAnalog()
    {
        hRotation = Quaternion.Euler(0f, (float)timeAnalog.TotalHours * degreesPerHour, 0f);
        mRotation = Quaternion.Euler(0f, (float)timeAnalog.TotalMinutes* degreesPerMinute, 0f);
        sRotation = Quaternion.Euler(0f, (float)timeAnalog.TotalSeconds * degreesPerSecond, 0f);

        hoursArm.localRotation = hRotation;
        minutesArm.localRotation = mRotation;
        secondsArm.localRotation = sRotation;
    }

    private void UpdateDiscrete()
    {
        hRotation = Quaternion.Euler(0f, timeDiscrete.Hour * degreesPerHour, 0f);
        mRotation = Quaternion.Euler(0f, timeDiscrete.Minute * degreesPerMinute, 0f);
        sRotation = Quaternion.Euler(0f, timeDiscrete.Second * degreesPerSecond, 0f);

        hoursArm.rotation = hRotation;
        minutesArm.rotation = mRotation;
        secondsArm.rotation = sRotation;
    }
}
