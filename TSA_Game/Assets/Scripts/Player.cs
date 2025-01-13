using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float accel;
    public float jumpImpulse;
    public float gravity = 1f;
    public float coyoteTime;

    Rigidbody rb;
    bool grounded;

    bool terminal_velocity()
    {
        return rb.velocity.y <= -Mathf.Sqrt(2 * rb.mass * gravity / 0.5f);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.velocity += new Vector3(Input.GetAxis("Horizontal") * accel, (terminal_velocity() && !grounded) ? -gravity : 0, 0);

        if ((Input.GetAxis("Vertical") > 0 || Input.GetKeyDown("space")) && grounded)
        {
            rb.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Impulse);
        }

        print(String.Format("vel(x: {0}, y: {1}, z: {2}), grounded: {3}, terminal_velocity: ", rb.velocity.x, rb.velocity.y, rb.velocity.z, grounded));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") { 
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }
}
