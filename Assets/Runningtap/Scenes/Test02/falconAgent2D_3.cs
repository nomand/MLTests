﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class falconAgent2D_3 : Agent {

    public float AngleGoal = 1.1f;
    public float AngleThreshold = 30;
    Rigidbody rb;
    Thrust2D falcon;
    Material mat;
    Vector3 position;

    float previousAngleDiff;
    float currentAngleDiff;

    bool centering = false;
    float time;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        falcon = GetComponent<Thrust2D>();
        position = transform.position;
        mat = gameObject.GetComponent<Renderer>().material;
    }

    public override void AgentReset()
    {
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        float z = Random.Range(-AngleThreshold, AngleThreshold);
        transform.eulerAngles = new Vector3(0, 0, z);
        transform.position = position;
    }

    public override void CollectObservations()
    {
        AddVectorObs(transform.rotation.eulerAngles.z / 180f - 1f); //normalized angle
        AddVectorObs(currentAngleDiff/90f); //normalized angle difference
        AddVectorObs(rb.angularVelocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        currentAngleDiff = Vector3.Angle(transform.up, Vector3.up);

        if(currentAngleDiff < AngleGoal)
        {
            time += Time.deltaTime;
            mat.color = Color.yellow;
            centering = true;

            if (time > 5f)
            {
                AddReward(0.5f);
                time = 0;
                Done();
                centering = false;
                StartCoroutine(FlashColor(Color.green));
            }
        }
        else if(currentAngleDiff > AngleGoal && centering)
        {
            time = 0;
            centering = false;
            StartCoroutine(FlashColor(Color.red, 0.5f));
        }

        AddReward(-1f / agentParameters.maxStep);

        //reward if angle decreases
        if (currentAngleDiff < previousAngleDiff)
        {
            AddReward(0.001f);
        }
        else
        {
            AddReward(-0.002f);
        }

        //fail being upside down
        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0)) > 90f)
        {
            AddReward(-1f);
            StartCoroutine(FlashColor(Color.red));
            AgentReset();
        }

        doThrust(vectorAction);        
        previousAngleDiff = currentAngleDiff;
    }

    void doThrust(float[] act)
    {
        int thruster = 0;
        int action = Mathf.FloorToInt(act[0]);
        switch (action)
        {
            case 0:
                thruster = 0;
                break;
            case 1:
                thruster = 1;
                break;
            case 2:
                thruster = -1;
                break;
        }

        falcon.ThrustAUX(thruster);
    }

    IEnumerator FlashColor(Color c, float t = 1)
    {
        mat.color = c;
        yield return new WaitForSeconds(t);
        mat.color = Color.grey;
        yield return null;
    }
}
