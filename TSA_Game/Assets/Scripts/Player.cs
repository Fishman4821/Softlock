using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float accel;
    public float jumpImpulse;
    public float gravity = 1f;
    public float coyoteTime = 0.5f;
    public float dragCoefficient = 1f;

    Rigidbody rb;
    SpriteRenderer sr;
    bool grounded = false;
    float timeSinceGrounded = 0.0f;
    bool jumping = false;

    Vector2 DragForce()
    {
        /*
        Fd = 1/2 * p * v ^ 2 * Cd * A
        where:
            p = 1.293 (air density)
            v = velocity
            Cd = Drag Coefficient
            A = 0.5 (area)
        */
        return new Vector2(1.293f * Mathf.Pow(rb.velocity.x, 2) * Mathf.Sign(rb.velocity.x) * dragCoefficient * 0.5f * 0.5f,
                           1.293f * Mathf.Pow(rb.velocity.y, 2) * Mathf.Sign(rb.velocity.y) * dragCoefficient * 0.5f * 0.5f);
    }

    bool terminal_velocity()
    {
        return rb.velocity.y <= -(Mathf.Sqrt(2 * rb.mass * gravity / 0.5f));
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();
    }
    //float time = 0.0f;
    void FixedUpdate()
    {
        Vector2 dragForce = DragForce();
        rb.velocity += new Vector3(Input.GetAxis("Horizontal") * accel - dragForce.x, ((!grounded) ? -gravity : 0) - dragForce.y, 0);

        if ((Input.GetAxis("Vertical") > 0 || Input.GetKeyDown("space")) && (grounded || timeSinceGrounded > 0.0f) && !jumping)
        {
            rb.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Force);
            jumping = true;
        }
        if (timeSinceGrounded > 0.0f)
        {
            timeSinceGrounded -= Time.deltaTime;
        }
       
        if (Input.GetAxis("Horizontal") < 0)
        {
            sr.flipX = true;
        } 
        else if (Input.GetAxis("Horizontal") > 0)
        {
            sr.flipX = false;
        }

        print(String.Format("vel(x: {0}, y: {1}, z: {2}), grounded: {3}, timeSinceGrounded: {4}, jumping: {5}, terminal_velocity: {8}, dragForce(x: {6}, y: {7}), Horizontal: {9}", rb.velocity.x, rb.velocity.y, rb.velocity.z, grounded, timeSinceGrounded, jumping, dragForce.x, dragForce.y, terminal_velocity(), Input.GetAxis("Horizontal")));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") {
            jumping = false;
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            timeSinceGrounded = coyoteTime;
            grounded = false;
        }
    }
}
