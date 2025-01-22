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

    Rigidbody2D rb;
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
        return new Vector2(/*1.293f * */Mathf.Pow(rb.velocity.x, 2) * Mathf.Sign(rb.velocity.x) * dragCoefficient * 0.5f * 0.5f,
                           /*1.293f * */Mathf.Pow(rb.velocity.y, 2) * Mathf.Sign(rb.velocity.y) * dragCoefficient * 0.5f * 0.5f);
    }

    bool terminal_velocity()
    {
        return rb.velocity.y <= -(Mathf.Sqrt(2 * rb.mass * gravity / 0.5f));
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Vector2 dragForce = DragForce();
        rb.velocity += new Vector2(Input.GetAxis("Horizontal") * accel - dragForce.x, ((!grounded) ? -gravity : 0) - dragForce.y);

        if ((Input.GetAxis("Vertical") > 0 || Input.GetKeyDown("space")) && (grounded || timeSinceGrounded > 0.0f) && !jumping)
        {
            rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
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

        print(String.Format("vel(x: {0}, y: {1}), grounded: {3}, timeSinceGrounded: {4}, jumping: {5}, terminal_velocity: {8}, dragForce(x: {6}, y: {7}), Horizontal: {9}, Vertical: {2}", rb.velocity.x, rb.velocity.y, Input.GetAxis("Vertical"), grounded, timeSinceGrounded, jumping, dragForce.x, dragForce.y, terminal_velocity(), Input.GetAxis("Horizontal")));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") {
            jumping = false;
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            timeSinceGrounded = coyoteTime;
            grounded = false;
        }
    }
}
