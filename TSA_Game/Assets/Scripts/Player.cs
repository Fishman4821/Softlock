using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float accel;
    public float jumpImpulse;
    public float gravity = 1f;

    Rigidbody rb;
    SpriteRenderer sr;
    bool grounded = false;

    public Vector2 gravity_bezier_p0;
    public Vector2 gravity_bezier_p1;
    public Vector2 gravity_bezier_p2;
    public Vector2 gravity_bezier_p3;

    Vector2 lerp_vec2(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t));
    }

    float lerp_cubic_bezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        Vector2 q0, q1, q2;
        Vector2 r0, r1;

        q0 = lerp_vec2(p0, p1, t);
        q1 = lerp_vec2(p1, p2, t);
        q2 = lerp_vec2(p2, p3, t);

        r0 = lerp_vec2(q0, q1, t);
        r1 = lerp_vec2(q1, q2, t);

        return lerp_vec2(r0, r1, t).y;
    }

    bool terminal_velocity()
    {
        return rb.velocity.y <= -(Mathf.Sqrt(2 * rb.mass * gravity / 0.5f));
    }

    float falling_speed(float t)
    {
        return -lerp_cubic_bezier(gravity_bezier_p0, gravity_bezier_p1, gravity_bezier_p2, gravity_bezier_p3, t) * gravity;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();
    }
    //float time = 0.0f;
    void FixedUpdate()
    {
        rb.velocity += new Vector3(Input.GetAxis("Horizontal") * accel, (!grounded) ? ((terminal_velocity()) ? 0 : falling_speed()) : 0, 0);

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

        //transform.position = new Vector3(time * 4f, falling_speed(time) * 4f + 2, 1);
        //time += 0.01f;
        //if (time >= 1.0f)
        //{
        //    time = 0.0f;
        //}
        //print(String.Format("time: {0}, y: {1}", time, falling_speed(time)));

        print(String.Format("vel(x: {0}, y: {1}, z: {2}), grounded: {3}, falling_speed: {4}", rb.velocity.x, rb.velocity.y, rb.velocity.z, grounded, falling_speed(0.0f)));
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
