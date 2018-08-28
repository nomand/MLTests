using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FalconAgent03 : Agent
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

    float thrust0;
    float thrust1;
    float thrust2;
    float thrust3;

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
        //AddVectorObs(transform.rotation.eulerAngles.x);
        //AddVectorObs(transform.rotation.eulerAngles.y);
        //AddVectorObs(transform.rotation.eulerAngles.z);

        AddVectorObs(rb.angularVelocity.x);
        AddVectorObs(rb.angularVelocity.z);

        //AddVectorObs(Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0)));
        AddVectorObs(Vector3.Angle(new Vector3( transform.eulerAngles.x, 0f, 0f), Vector3.up));
        AddVectorObs(Vector3.Angle(new Vector3( 0f, 0f, transform.eulerAngles.z), Vector3.up));
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

        thrust0 = vectorAction[0];
        thrust1 = vectorAction[1];
        thrust2 = vectorAction[2];
        thrust3 = vectorAction[3];

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