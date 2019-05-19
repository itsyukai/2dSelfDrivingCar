using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DrivingAgent : Agent
{
    Rigidbody2D rb;
    public Transform Target;

    // For Car Controls 
    private float acceleration = 5f;  // 5
    private float steering = 5f;      // 3

    private float steeringAmount, speed, direction;


    bool m_Started;

    private RayPerception rayPercept; 

    // Ray Casting Points
    public Transform topPoint;
    public Transform bottomPoint;
    public Transform rightPoint;
    public Transform leftPoint;
    public Transform TLPoint;
    public Transform TRPoint;
    public Transform BLPoint;
    public Transform BRPoint;

    public int testNumber = 0;


    // Start is called before the first frame update
    void Start()
    {
        m_Started = true;
        rb = GetComponent<Rigidbody2D>();
    }

    public override void AgentReset()
    {
        if (testNumber == 0) {
            transform.position = new Vector3(2.53f, -6.87f, transform.position.z);
            Target.position = new Vector3(2.53f, Random.Range(-6, 6), Target.position.z);
        }
    }
    public override void CollectObservations()
    {
        // Steering
        AddVectorObs(speed);
        AddVectorObs(steeringAmount);
        AddVectorObs(direction);

        // Positions
        AddVectorObs(Target.position);
        AddVectorObs(transform.position);

        // Ray Cast Direction  
        Vector3 rDir = transform.rotation * Vector2.right;
        Vector3 lDir = transform.rotation * -Vector2.right;
        Vector3 tDir = transform.rotation * Vector2.up;
        Vector3 bDir = transform.rotation * -Vector2.up;
        Vector3 tlDir = transform.rotation * Vector2.up.Rotate(45);
        Vector3 trDir = transform.rotation * Vector2.right.Rotate(45);
        Vector3 blDir = transform.rotation * -Vector2.right.Rotate(45);
        Vector3 brDir = transform.rotation * -Vector2.up.Rotate(45);

        // Ray Casts
        RaycastHit2D hitLeft = Physics2D.Raycast(leftPoint.position, lDir);
        RaycastHit2D hitRight = Physics2D.Raycast(rightPoint.position, rDir);
        RaycastHit2D hitBottom = Physics2D.Raycast(bottomPoint.position, bDir);
        RaycastHit2D hitTop = Physics2D.Raycast(topPoint.position, tDir);
        RaycastHit2D hitTL = Physics2D.Raycast(TLPoint.position, tlDir);
        RaycastHit2D hitTR = Physics2D.Raycast(TRPoint.position, trDir);
        RaycastHit2D hitBL = Physics2D.Raycast(BLPoint.position, blDir);
        RaycastHit2D hitBR = Physics2D.Raycast(BRPoint.position, brDir);

        // Distance of RayCast Hits Initialized to 999 in case of no hit
        float distanceLeft = 999;
        float distanceRight = 999;
        float distanceTop = 999;
        float distanceBottom = 999;
        float distanceTL = 999;
        float distanceTR = 999;
        float distanceBL = 999;
        float distanceBR = 999;

        if (hitLeft.collider != null)
            distanceLeft = Vector2.Distance(leftPoint.position, hitLeft.point);
        if (hitRight.collider != null)
            distanceRight = Vector2.Distance(rightPoint.position, hitRight.point);
        if (hitTop.collider != null)
            distanceTop = Vector2.Distance(topPoint.position, hitLeft.point);
        if (hitBottom.collider != null)
            distanceBottom = Vector2.Distance(leftPoint.position, hitLeft.point);
        if (hitTL.collider != null)
            distanceTL = Vector2.Distance(TLPoint.position, hitTL.point);
        if (hitTR.collider != null)
            distanceTR = Vector2.Distance(TRPoint.position, hitTR.point);
        if (hitBL.collider != null)
            distanceBL = Vector2.Distance(BLPoint.position, hitBL.point);
        if (hitBR.collider != null)
            distanceBR = Vector2.Distance(BRPoint.position, hitBR.point);

        AddVectorObs(distanceLeft);
        AddVectorObs(distanceRight);
        AddVectorObs(distanceTop);
        AddVectorObs(distanceBottom);
        AddVectorObs(distanceTL);
        AddVectorObs(distanceTR);
        AddVectorObs(distanceBL);
        AddVectorObs(distanceBR);


    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        steeringAmount = vectorAction[0];
        speed = vectorAction[1] * acceleration;

        direction = Mathf.Sign(Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up)));

        rb.rotation += steeringAmount * steering * rb.velocity.magnitude * direction;

        rb.AddRelativeForce(-Vector2.up * speed);

        rb.AddRelativeForce(-Vector2.right * rb.velocity.magnitude * steeringAmount / 2);




        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);
        // Reached target
        if (distanceToTarget < 0.2f && System.Math.Abs(rb.velocity.sqrMagnitude) < 0.2f)
        {
            SetReward(1.0f);
            Done();
        }
    }


    //draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
        {
            // Ray Cast Angles
            Vector3 rDir = transform.rotation * Vector2.right;
            Vector3 lDir = transform.rotation * -Vector2.right;
            Vector3 tDir = transform.rotation * Vector2.up;
            Vector3 bDir = transform.rotation * -Vector2.up;
            Vector3 tlDir = transform.rotation * Vector2.up.Rotate(45);
            Vector3 trDir = transform.rotation * Vector2.right.Rotate(45);
            Vector3 blDir = transform.rotation * -Vector2.right.Rotate(45);
            Vector3 brDir = transform.rotation * -Vector2.up.Rotate(45);

            // preview ray casts
            Gizmos.DrawRay(leftPoint.position, lDir);
            Gizmos.DrawRay(rightPoint.position, rDir);
            Gizmos.DrawRay(bottomPoint.position, bDir);
            Gizmos.DrawRay(topPoint.position, tDir);
            Gizmos.DrawRay(TLPoint.position, tlDir);
            Gizmos.DrawRay(TRPoint.position, trDir);
            Gizmos.DrawRay(BLPoint.position, blDir);
            Gizmos.DrawRay(BRPoint.position, brDir);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Buildings"))
        {
            SetReward(-1.0f);
            Done();
        }
    }
}
