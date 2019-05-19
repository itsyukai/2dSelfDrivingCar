using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DrivingAgent2 : Agent
{
    Rigidbody2D rb;
    public GameObject Target;
    public Transform[] targetPositions;
    public DrivingSchool academy;

    // For Car Controls 
    private float acceleration = 5f;  // 5
    private float steering = 5f;      // 3

    private float steeringAmount, speed, direction;

    private float rayDistance = 6.0f;


    //bool m_Started;

    private RayPerception2D rayPercept;


    public int configuration = 0;


    // Start is called before the first frame update
    void Start()
    {
        //m_Started = true;
        rb = GetComponent<Rigidbody2D>();
        rayPercept = GetComponent<RayPerception2D>();
    }

    private void FixedUpdate()
    {
        //if (configuration != -1)
        //{
        //    ConfigureAgent(configuration);
        //    configuration = -1;
        //}
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(1.8f, -4.43f, transform.position.z);
        }
    }

    /// <summary>
    /// Configures the agent. Given an integer config,the environment will
    /// have different obstacles and different brain assigned to agent
    /// </summary>
    /// <param name="config">Config. 
    /// If 0 : Straight Line, straightBrain
    /// If 1:  Turns, turnBrain
    //void ConfigureAgent(int config) {
    //    if (config == 0)
    //    {
    //        wall.transform.localScale = new Vector3(
    //            wall.transform.localScale.x,
    //            academy.resetParameters["no_wall_height"],
    //            wall.transform.localScale.z);
    //        GiveBrain(noWallBrain);
    //    }
    //}

    public override void AgentReset()
    {
        Debug.Log("Reseting");
        if (configuration == 0)
        {
            Target.transform.position = targetPositions[(int)academy.resetParameters["target_locations"]].position;
            //Debug.Log(academy.resetParameters[""])
            //Target.transform.position = new Vector3(1.81f, -1.14f, transform.position.z);
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(1.8f, -4.43f, transform.position.z);
            //Target.position = new Vector3(2.53f, Random.Range(-6, 6), Target.position.z);
        }
    }
    public override void CollectObservations()
    {
        // Steering
        AddVectorObs(speed);
        AddVectorObs(steeringAmount);
        AddVectorObs(direction);

        // Positions
        AddVectorObs(Target.transform.position);
        AddVectorObs(transform.position);


        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 225, 270f, 315 };
        //string[] detectableObjects;
        string[] detectableObjects = new string[] {"Target","Wall"};
        AddVectorObs(rayPercept.Perceive(rayDistance, rayAngles, detectableObjects));

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        AddReward(-0.0005f);
        steeringAmount = vectorAction[0];
        speed = vectorAction[1] * acceleration;

        direction = Mathf.Sign(Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up)));

        rb.rotation += steeringAmount * steering * rb.velocity.magnitude * direction;

        rb.AddRelativeForce(-Vector2.up * speed);

        rb.AddRelativeForce(-Vector2.right * rb.velocity.magnitude * steeringAmount / 2);


        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.transform.position);
        // Reached target
        if (distanceToTarget < 1.0f)
        {
            SetReward(1.0f);
            Done();
        }

        RaycastHit2D hitRight, hitLeft;
        Vector2 endPosition = transform.TransformDirection( PolarToCartesian(1, 90));
        hitRight = Physics2D.CircleCast(transform.position, 0.5f, endPosition, 1);
        Vector2 endPosition2 = transform.TransformDirection(PolarToCartesian(1, 90));
        hitLeft = Physics2D.CircleCast(transform.position, 0.5f, endPosition, 1);

        if (Application.isEditor)
        {
            Debug.DrawRay(transform.position,
                endPosition, Color.red, 0.01f, true);
            Debug.DrawRay(transform.position,
                endPosition2, Color.red, 0.01f, true);
        }
        if (hitRight.collider != null &&
            hitLeft.collider != null &&
            hitLeft.collider.CompareTag("Wall") &&
            hitRight.collider.CompareTag("Wall"))
        {
            AddReward( (1 - hitRight.distance ) * 0.1f);
            AddReward( (1 - hitLeft.distance ) * 0.1f);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger Detected");
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Trigger a wall");
            SetReward(-1.0f);
            Done();
        }
        //if (collision.gameObject.CompareTag("Solid Line"))
        //{
        //    Debug.Log("Trigger a Solid Line");
        //    AddReward(-0.05f);
        //}

    }

    public override void AgentOnDone()
    {
        Debug.Log("Done Called");
    }


    /// <summary>
    /// Taken from RayPerception2D
    /// Converts polar coordinate to cartesian coordinate.
    /// </summary>
    public static Vector2 PolarToCartesian(float radius, float angle)
    {
        float x = radius * Mathf.Cos(DegreeToRadian(angle));
        float y = radius * Mathf.Sin(DegreeToRadian(angle));
        return new Vector2(x, y);
    }

    /// <summary>
    /// Taken from RayPerception2D
    /// Converts degrees to radians.
    /// </summary>
    public static float DegreeToRadian(float degree)
    {
        return degree * Mathf.PI / 180f;
    }
}
