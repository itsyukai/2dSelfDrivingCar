using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControls : MonoBehaviour
{

    private float acceleration = 5f;  // 5
    private float steering = 5f;      // 3

    private float steeringAmount, speed, direction;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        steeringAmount = Input.GetAxis("Horizontal");
        speed = Input.GetAxis("Vertical") * acceleration;

        direction = Mathf.Sign(Vector2.Dot (rb.velocity, rb.GetRelativeVector(Vector2.up)));
        
        rb.rotation += steeringAmount * steering * rb.velocity.magnitude * direction;

        rb.AddRelativeForce( - Vector2.up * speed);

        rb.AddRelativeForce( - Vector2.right * rb.velocity.magnitude * steeringAmount / 2);
    }
}
