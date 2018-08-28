using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Test01Agent3 : Agent
{
    public Transform Ball;
    public Transform Set;
    public BoxCollider inputBound;

    public float speed = 3f;
    public float turnSpeed = 5f;
    Rigidbody rb;
    Rigidbody ballrb;
    RayPerception rp;
    Bounds bound;

    float forward;
    float rotation;
    float currentDistance;

    Material mat;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballrb = Ball.GetComponent<Rigidbody>();
        rp = GetComponent<RayPerception>();
        bound = inputBound.bounds;
        mat = Set.GetComponent<Renderer>().material;
    }

    public override void AgentReset()
    {
        transform.position = new Vector3(Set.transform.position.x - Random.Range(-9.5f, 9.5f), 0.5f, Set.transform.position.z - Random.Range(-9.5f, 9.5f));
        Ball.transform.position = new Vector3(Set.transform.position.x - Random.Range(-9.5f, 9.5f), 0.5f, Set.transform.position.z - Random.Range(-9.5f, 9.5f));

        transform.Rotate(new Vector3(0, Random.Range(-180, 180), 0));
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ballrb.velocity = Vector3.zero;
        ballrb.angularVelocity = Vector3.zero;
    }

    bool BoundsContain(Transform t)
    {
        if(bound.Contains(t.position))
            return true;
        else
            return false;
    }

    public override void CollectObservations()
    {
        var rayDistance = 10f;
        float[] rayAngles = { 90f };
        var detectableObjects = new[] { "ball", "wall" };
        AddVectorObs(rp.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        currentDistance = Vector3.Distance(transform.position, Ball.transform.position);

        //reward if you get the ball out of bounds
        if (!BoundsContain(Ball))
        {
            AddReward(1f);
            StartCoroutine(Success(Color.green));
            Done();
        }

        //penalty if you go out of bounds
        if(!BoundsContain(transform))
        {
            AddReward(-1f);
            StartCoroutine(Success(Color.red));
            AgentReset();
        }

        //hurry up
        AddReward(-1f / agentParameters.maxStep);

        rotation += vectorAction[0] * turnSpeed;
        Vector3 newHeading = new Vector3(transform.eulerAngles.x, rotation, transform.eulerAngles.z);

        rb.MoveRotation(Quaternion.Euler(newHeading));
        rb.AddForce(transform.forward * vectorAction[1] * speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == Ball.gameObject)
        {
            AddReward(collision.relativeVelocity.magnitude * 0.1f);
        }
    }

    IEnumerator Success(Color c)
    {
        mat.color = c;
        yield return new WaitForSeconds(1);
        mat.color = Color.grey;
        yield return null;
    }
}