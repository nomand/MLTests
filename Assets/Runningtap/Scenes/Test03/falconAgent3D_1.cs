using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class falconAgent3D_1 : Agent {

    public float AngleGoal = 1.1f;
    public float AngleThreshold = 30;
    Rigidbody rb;
    Thrust3D falcon;
    Material mat;
    Vector3 position;

    Vector3 PreviousRotation;

    bool centering = false;
    float time;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        falcon = GetComponent<Thrust3D>();
        position = transform.position;
        mat = gameObject.GetComponent<Renderer>().material;
    }

    public override void AgentReset()
    {
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        float z = Random.Range(-AngleThreshold, AngleThreshold);
        float x = Random.Range(-AngleThreshold, AngleThreshold);
        transform.eulerAngles = new Vector3(x, 0, z);
        transform.position = position;
    }

    public override void CollectObservations()
    {
        AddVectorObs((Vector3.Angle(transform.right, Vector3.up) - 90) / 180f - 1f); //normalized angle
        AddVectorObs((Vector3.Angle(transform.forward, Vector3.up) - 90) / 180f - 1f); //normalized angle

        AddVectorObs(rb.angularVelocity.x);
        AddVectorObs(rb.angularVelocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float z = Mathf.Abs(Vector3.Angle(transform.right, Vector3.up) - 90);
        float x = Mathf.Abs(Vector3.Angle(transform.forward, Vector3.up) - 90);

        if (z < AngleGoal && x < AngleGoal)
        {
            AddReward(0.0001f);
            time += Time.deltaTime;
            mat.color = Color.yellow;
            centering = true;

            if (time > 5f)
            {
                AddReward(1f);
                time = 0;
                Done();
                centering = false;
                StartCoroutine(FlashColor(Color.green));
            }
        }

        else if (z > AngleGoal && centering)
        {
            time = 0;
            centering = false;
            StartCoroutine(FlashColor(Color.red, 0.5f));
        }

        AddReward(-1f / agentParameters.maxStep);

        //reward if angle decreases
        if (z < PreviousRotation.z)
            AddReward(0.001f);
        else
            AddReward(-0.002f);

        if (x < PreviousRotation.x)
            AddReward(0.001f);
        else
            AddReward(-0.002f);

        //fail being upside down
        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 1, 0)) > 90f)
        {
            AddReward(-1f);
            StartCoroutine(FlashColor(Color.red));
            AgentReset();
        }

        doThrust(vectorAction);
        PreviousRotation = new Vector3(x, 0f, z);
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
            case 3:
                thruster = 2;
                break;
            case 4:
                thruster = -2;
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
