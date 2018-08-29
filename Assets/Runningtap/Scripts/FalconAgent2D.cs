using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FalconAgent2D : Agent
{
    public float AngleGoal = 1.1f;
    public float AngleThreshold = 30;
    Thrust falcon;
    Transform body;
    Rigidbody rb;
    Vector3 position;

    float previousAngleDiff;
    float currentAngleDiff;

    float currentAngularVelocity;
    float previousAngularVelocity;

    Material mat;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        falcon = GetComponent<Thrust>();
        position = transform.position;
        mat = gameObject.GetComponent<Renderer>().material;
    }

    public override void AgentReset()
    {
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        float x = Random.Range(-AngleThreshold, AngleThreshold);
        float y = Random.Range(-AngleThreshold, AngleThreshold);
        float z = Random.Range(-AngleThreshold, AngleThreshold);
        transform.eulerAngles = new Vector3(x, y, z);
        transform.position = position;
    }

    public override void CollectObservations()
    {
        AddVectorObs(rb.angularVelocity.z);
        AddVectorObs(Vector3.Angle(Vector3.up, transform.right));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        currentAngleDiff = Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0));
        currentAngularVelocity = Mathf.Abs(rb.angularVelocity.magnitude);

        //reward completion
        if (currentAngleDiff < AngleGoal)
        {
            AddReward(1);
            StartCoroutine(Success(Color.green));
            Done();
        }

        //reward for slowing down rotation
        if (currentAngularVelocity < previousAngularVelocity)
        {
            AddReward(0.01f);
        }

        //punish if angle increases
        if (currentAngleDiff > previousAngleDiff)
        {
            AddReward(-0.01f);
        }
        
        //punish being upside down
        if(Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0)) > 90f)
        {
            AddReward(-1f);
            StartCoroutine(Success(Color.red));
            AgentReset();
        }

        //punish for spinning too fast
        //AddReward(currentAngularVelocity * -10f);

        //punish for angle difference
        //AddReward(Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0))) * -0.01f);

        //punish over time
        AddReward(-1f / agentParameters.maxStep);

        float[] control = { vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3] };
        falcon.ThrustAUX(control);

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