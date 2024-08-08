using System.Collections;
using System.Collections.Generic;
using Baracuda.Monitoring;
using UnityEngine;

public class MovementController : MonoBehaviour {
    public Vector2 Velocity { get; private set; } = Vector2.zero;
    public Vector3 Velocity3 => Velocity; // might be redundant
    public float Speed => Velocity.magnitude;
    public float maxSpeed = 3f;
    public float maxAcceleration = 3f;

    private Rigidbody2D _rb;

    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate() {
        CalculateVelocity();
        ApplyDisplacement();
    }
    
    void CalculateVelocity() {
        var originalInput = new Vector2 {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };
        
        var playerInput = originalInput;
        float minInput = 0.9999f;
        playerInput.x = Mathf.Abs(playerInput.x) < minInput ? 0 : Mathf.Sign(playerInput.x);
        playerInput.y = Mathf.Abs(playerInput.y) < minInput ? 0 : Mathf.Sign(playerInput.y);
        
        playerInput.Normalize();
        
        // float inputDeadzone = 0.01f;
        // if (Mathf.Abs(playerInput.x) < 1f && Mathf.Abs(playerInput.y) < 1f) {
        //     Velocity = Vector2.zero;
        //     //Debug.Log("No input, velocity killed.");
        //     return;
        // }
        
        // Vector2 desiredVelocity = playerInput * maxSpeed;
        // float maxSpeedChange = maxAcceleration * Time.deltaTime;

        // var xVel = Mathf.MoveTowards(Velocity.x, desiredVelocity.x, maxSpeedChange);
        // var yVel = Mathf.MoveTowards(Velocity.y, desiredVelocity.y, maxSpeedChange);

        //Velocity = new Vector2(xVel, yVel); 
        Velocity = playerInput * maxSpeed;
        // Debug.Log(originalInput + " -> " + playerInput + " -> " + Velocity);
    }
    
    void ApplyDisplacement() {
        // Vector3 displacement = Velocity * Time.deltaTime;
        // transform.localPosition += displacement;
        _rb.velocity = Velocity;
    }
}