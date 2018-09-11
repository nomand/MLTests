﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrust3D : MonoBehaviour
{

    public float ThrustVector;
    public Transform[] BoosterAUX;
    public ParticleSystem[] VisualAUX;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ThrustAUX(int thrust)
    {
        if(thrust == 1)
        {
            rb.AddForceAtPosition(BoosterAUX[0].TransformDirection(Vector3.back) * ThrustVector * thrust, BoosterAUX[0].position, ForceMode.Force);
            VisualAUX[0].Play();
        }
        else if(thrust == -1)
        {
            rb.AddForceAtPosition(BoosterAUX[1].TransformDirection(Vector3.forward) * ThrustVector * thrust, BoosterAUX[1].position, ForceMode.Force);
            VisualAUX[1].Play();
        }
        else if (thrust == 2)
        {
            rb.AddForceAtPosition(BoosterAUX[2].TransformDirection(Vector3.back) * ThrustVector * thrust, BoosterAUX[2].position, ForceMode.Force);
            VisualAUX[2].Play();
        }
        else if (thrust == -2)
        {
            rb.AddForceAtPosition(BoosterAUX[3].TransformDirection(Vector3.forward) * ThrustVector * thrust, BoosterAUX[3].position, ForceMode.Force);
            VisualAUX[3].Play();
        }
    }
}