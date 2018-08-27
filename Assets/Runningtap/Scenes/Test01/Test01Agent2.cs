using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Test01Agent2 : Agent
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

    float rotation;
    float action1;
    Vector3 newHeading;

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
        transform.position = new Vector3(Set.transform.position.x - Random.Range(-bound.extents.x, bound.extents.x), 0.5f, Set.transform.position.z - Random.Range(-bound.extents.z, bound.extents.z));
        Ball.transform.position = new Vector3(Set.transform.position.x - Random.Range(-bound.extents.x, bound.extents.x), 0.5f, Set.transform.position.z - Random.Range(-bound.extents.z, bound.extents.z));

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
        float[] rayAngles = { 90f };
        var detectableObjects = new[] { "ball", "wall" };
        AddVectorObs(rp.Perceive(10f, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rp.Perceive(2f, new[] { 0f, 180f, 270f }, new[] {"wall"}, 0f, 0f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
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
        action1 = vectorAction[1];
    }

    void FixedUpdate()
    {
        newHeading = new Vector3(0f, rotation, 0f);
        rb.MoveRotation(Quaternion.Euler(newHeading));
        rb.AddForce(transform.forward * action1 * speed, ForceMode.VelocityChange);
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