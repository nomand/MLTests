using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrust : MonoBehaviour {

    public float ThrustMain;
    public float ThrustVector;

    public Transform[] BoosterAUX;
    public ParticleSystem[] VisualAUX;

    Rigidbody rb;

    bool FireMain;
    bool FireLeft;
    bool FireRight;
    bool FireUp;
    bool FireDown;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }
	
	void Update ()
    {

    }

    public void ThrustAUX(float[] thrust)
    {
        for (int i = 0; i < 4; i++)
        {
            rb.AddForceAtPosition(BoosterAUX[i].TransformDirection(Vector3.back) * ThrustVector * thrust[i], BoosterAUX[i].position, ForceMode.Force);
            if(thrust[i] > 0) VisualAUX[i].Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(KineticEnergy(rb) > 1000)
        {
            print(KineticEnergy(rb) + "dead");
        }
        else { print(KineticEnergy(rb) + "success"); }
    }

    public static float KineticEnergy(Rigidbody rb)
    {
        // mass in kg, velocity in meters per second, result is joules
        return 0.5f * rb.mass * Mathf.Pow(rb.velocity.magnitude, 2);
    }
}