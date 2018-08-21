using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FalconAgent : Agent
{
    public float AngleDiff = 0.01f;
    public float AngleThreshold = 30;
    Thrust falcon;
    Transform body;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        falcon = GetComponent<Thrust>();
    }

    public override void AgentReset()
    {
        if (Vector3.Angle(this.transform.rotation.eulerAngles, Vector3.up) < AngleDiff)
        {
            // The agent is up right
            this.transform.rotation = Quaternion.identity;
            this.rb.angularVelocity = Vector3.zero;
            this.rb.velocity = Vector3.zero;
        }
        else
        {
            // Set random rotation
            float x = Random.Range(-AngleThreshold, AngleThreshold);
            float y = Random.Range(-AngleThreshold, AngleThreshold);
            float z = Random.Range(-AngleThreshold, AngleThreshold);
            transform.eulerAngles = new Vector3(x, y, z);
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(this.transform.rotation.eulerAngles.x);
        AddVectorObs(this.transform.rotation.eulerAngles.y);
        AddVectorObs(this.transform.rotation.eulerAngles.z);

        AddVectorObs(this.rb.angularVelocity.x);
        AddVectorObs(this.rb.angularVelocity.y);
        AddVectorObs(this.rb.angularVelocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //time penalty
        AddReward(-0.01f);

        //goal
        float currentAngleDiff = Vector3.Angle(this.transform.rotation.eulerAngles, Vector3.up);

        //target reached
        if(Vector3.Angle(this.transform.rotation.eulerAngles, Vector3.up) < currentAngleDiff)
        {
            AddReward(2.0f);
            Done();
        }

        //avoid being beyond threshold
        if(Vector3.Angle(this.transform.rotation.eulerAngles, Vector3.up) > AngleThreshold)
        {
            AddReward(-0.5f);
        }

        if(rb.angularVelocity.magnitude > 1f)
        {
            AddReward(-0.5f);
        }

        float[] control = { vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3] };
        falcon.ThrustAUX(control);
    }
}