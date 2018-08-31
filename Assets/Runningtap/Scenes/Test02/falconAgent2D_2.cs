using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class falconAgent2D_2 : Agent {

    public float AngleGoal = 1.1f;
    public float AngleThreshold = 30;
    Rigidbody rb;
    Thrust2D falcon;
    Material mat;
    Vector3 position;

    float previousAngleDiff;
    float currentAngleDiff;

    bool centering;
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
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        currentAngleDiff = Vector3.Angle(transform.up, Vector3.up);

        if(currentAngleDiff < AngleGoal)
        {
            time += Time.deltaTime;
            mat.color = Color.yellow;

            if (time > 3)
            {
                AddReward(0.5f);
                StartCoroutine(FlashColor(Color.green));
                Done();
            }
        }
        else
        {
            AddReward(-1f / agentParameters.maxStep);
            time = 0;
            StartCoroutine(FlashColor(Color.red, 0.5f));
        }

        ////reward if angle decreases
        //if (currentAngleDiff < previousAngleDiff)
        //{
        //    AddReward(0.01f);
        //}

        ////punish if angle increases
        //if (currentAngleDiff > previousAngleDiff)
        //{
        //    AddReward(-0.02f);
        //}

        //fail being upside down
        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0)) > 90f)
        {
            AddReward(-1f);
            StartCoroutine(FlashColor(Color.red));
            AgentReset();
        }

        //punish over time
        //AddReward(-1f / agentParameters.maxStep);

        falcon.ThrustAUX(vectorAction[0]);
        previousAngleDiff = currentAngleDiff;
    }

    IEnumerator FlashColor(Color c, float t = 1)
    {
        mat.color = c;
        yield return new WaitForSeconds(t);
        mat.color = Color.grey;
        yield return null;
    }
}
