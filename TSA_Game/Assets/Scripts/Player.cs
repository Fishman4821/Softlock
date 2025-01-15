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
    SpriteRenderer sr;
    bool grounded = false;

    bool terminal_velocity()
    {
        return rb.velocity.y <= -(Mathf.Sqrt(2 * rb.mass * gravity / 0.5f));
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        rb.velocity += new Vector3(Input.GetAxis("Horizontal") * accel, (!grounded) ? ((terminal_velocity()) ? 0 : -gravity) : 0, 0);

        if ((Input.GetAxis("Vertical") > 0 || Input.GetKeyDown("space")) && grounded)
        {
            rb.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Impulse);
        }
       
        if (Input.GetAxis("Horizontal") < 0)
        {
            sr.flipX = true;
        } 
        else if (Input.GetAxis("Horizontal") > 0)
        {
            sr.flipX = false;
        }

        print(String.Format("vel(x: {0}, y: {1}, z: {2}), grounded: {3}, terminal_velocity: {4}, {5}", rb.velocity.x, rb.velocity.y, rb.velocity.z, grounded, terminal_velocity(), Mathf.Sqrt(2 * rb.mass * gravity / 0.5f)));
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
