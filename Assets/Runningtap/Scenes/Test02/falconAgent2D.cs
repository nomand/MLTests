﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class falconAgent2D : Agent {

    public float AngleGoal = 1.1f;
    public float AngleThreshold = 30;
    Rigidbody rb;
    Thrust2D falcon;
    Material mat;
    Vector3 position;

    float previousAngleDiff;
    float currentAngleDiff;

    float currentAngularVelocity;
    float previousAngularVelocity;

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
        AddVectorObs(transform.eulerAngles.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        currentAngularVelocity = Mathf.Abs(rb.angularVelocity.magnitude);
        currentAngleDiff = Vector3.Angle(transform.up, Vector3.up);

        if(currentAngleDiff < AngleGoal)
        {
            AddReward(1);
            StartCoroutine(Success(Color.green));
            Done();
        }

        //punish if angle increases
        if (currentAngleDiff < previousAngleDiff)
        {
            AddReward(0.01f);
        }

        //punish if angle increases
        if (currentAngleDiff > previousAngleDiff)
        {
            AddReward(-0.05f);
        }

        //fail being upside down
        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0)) > 90f)
        {
            AddReward(-1f);
            StartCoroutine(Success(Color.red));
            AgentReset();
        }

        //punish over time
        AddReward(-1f / agentParameters.maxStep);

        falcon.ThrustAUX(vectorAction[0]);
        previousAngleDiff = currentAngleDiff;
        previousAngularVelocity = currentAngularVelocity;
    }

    IEnumerator Success(Color c)
    {
        mat.color = c;
        yield return new WaitForSeconds(1);
        mat.color = Color.grey;
        yield return null;
    }
}
