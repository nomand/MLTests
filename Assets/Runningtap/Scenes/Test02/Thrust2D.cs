using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrust2D : MonoBehaviour {

    public float ThrustVector;
    public Transform[] BoosterAUX;
    public ParticleSystem[] VisualAUX;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ThrustAUX(float thrust)
    {
        if(thrust > 0)
        {
            rb.AddForceAtPosition(BoosterAUX[0].TransformDirection(Vector3.back) * ThrustVector * thrust, BoosterAUX[0].position, ForceMode.Force);
            VisualAUX[0].Play();
        }
        else if(thrust < 0)
        {
            rb.AddForceAtPosition(BoosterAUX[1].TransformDirection(Vector3.forward) * ThrustVector * thrust, BoosterAUX[1].position, ForceMode.Force);
            VisualAUX[1].Play();
        }
    }
}