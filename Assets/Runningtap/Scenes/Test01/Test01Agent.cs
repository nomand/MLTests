using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Test01Agent : Agent
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
    float previousDistance;

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

    bool isOutOfBounds(Transform t)
    {
        if(bound.Contains(t.position))
            return true;
        else
            return false;
    }

    public override void CollectObservations()
    {
        AddVectorObs(Ball.position.x - transform.position.x);
        AddVectorObs(Ball.position.z - transform.position.z);

        AddVectorObs(bound.extents.x - transform.position.x);
        AddVectorObs(bound.extents.z - transform.position.z);

        AddVectorObs(transform.position.x);
        AddVectorObs(transform.position.z);
        AddVectorObs(transform.eulerAngles.y);

        AddVectorObs(Ball.transform.position.x);
        AddVectorObs(Ball.transform.position.y);
        AddVectorObs(Ball.transform.position.z);

        AddVectorObs(rb.velocity.x);
        AddVectorObs(rb.velocity.z);
        AddVectorObs(currentDistance);

        var rayDistance = 12f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 225f, 280f };
        var detectableObjects = new[] { "ball" };
        AddVectorObs(rp.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rp.Perceive(rayDistance, rayAngles, detectableObjects, 1.5f, 0f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        currentDistance = Vector3.Distance(transform.position, Ball.transform.position);

        if (!isOutOfBounds(Ball))
        {
            AddReward(1f);
            StartCoroutine(Success(Color.green));
            Done();
        }

        if(!isOutOfBounds(transform))
        {
            AddReward(-1f);
            StartCoroutine(Success(Color.red));
            AgentReset();
        }

        //if(currentDistance < previousDistance)
        //{
        //    AddReward(0.1f);
        //}
        //else
        //{
        //    AddReward(-0.2f);
        //}

        AddReward(-1f / agentParameters.maxStep);

        rotation += vectorAction[0] * turnSpeed;
        Vector3 newHeading = new Vector3(transform.eulerAngles.x, rotation, transform.eulerAngles.z);

        rb.MoveRotation(Quaternion.Euler(newHeading));
        rb.AddForce(transform.forward * vectorAction[1] * speed, ForceMode.VelocityChange);

        previousDistance = currentDistance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == Ball.gameObject)
        {
            AddReward(0.1f);
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