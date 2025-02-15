using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    public float accel;
    public float jumpImpulse;
    public float gravity = 1f;
    public float coyoteTime = 0.5f;
    public float dragCoefficient = 1f;
    public float groundFriction = 1f;
    public float airAccelMult = 1f;
    public Transform cameraT;
    public Transform backgroundT;
    public float smoothTime = 0.3F;
    public int maxAirDashes = 2;
    public float dashDistance = 5f;
    public float dashTime = 0.05f;
    public float dashCooldown = 0.2f;
    public Vector2 wallJumpForce;

    public CoinManager cm;

    public Sprite normalSprite;
    public Sprite dashingSprite;
    public Sprite dyingSprite;

    public List<BoxCollider2D> dashWalls;

    private Collider2D collider;
    private Vector2 respawnPoint;

    Rigidbody2D rb;
    SpriteRenderer sr;
    bool grounded = false;
    float timeSinceGrounded = 0.0f;
    bool jumping = false;
    bool spacePressed = false;
    bool shiftPressed = false;
    int airDashes = 0;
    bool canDash = true;

    [SerializeField] private TrailRenderer tr;
    [SerializeField] private bool active = true;

    int horizontalLastHeldDirection = 0;
    bool dashNoGravity = false;

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
        tr.emitting = false;
        collider = GetComponent<Collider2D>();
        SetRespawnPoint(transform.position);
    }
    Vector3 velocity = new Vector3(0, 0, 0);
    private void Update()
    {
        Vector3 targetPosition = transform.position + new Vector3(0, 0, -10.0f);
        cameraT.position = Vector3.SmoothDamp(cameraT.position, targetPosition, ref velocity, smoothTime);
        backgroundT.position = new Vector3(cameraT.position.x, cameraT.position.y, 1);

        spacePressed = Input.GetKey("space") || Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow);
        shiftPressed = Input.GetKey(KeyCode.LeftShift);
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal < 0)
        {
            horizontalLastHeldDirection = -1;
        } else if (horizontal > 0)
        {
            horizontalLastHeldDirection = 1;
        }
        if (grounded) { jumping = false; }
    }

    void FixedUpdate()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D groundedRay1 = Physics2D.Raycast(pos + new Vector2(-0.5f, -0.5f), new Vector2(0, -1), 0.05f);
        RaycastHit2D groundedRay2 = Physics2D.Raycast(pos + new Vector2(0.5f, -0.5f), new Vector2(0, -1), 0.05f);
        Debug.DrawLine(transform.position + new Vector3(-0.5f, -0.5f), transform.position + new Vector3(-0.5f, -0.55f), groundedRay1 ? Color.blue : Color.red, 0.05f, true);
        Debug.DrawLine(transform.position + new Vector3(0.5f, -0.5f), transform.position + new Vector3(0.5f, -0.55f), groundedRay2 ? Color.blue : Color.red, 0.05f, true);

        if (groundedRay1 || groundedRay2)
        {
            airDashes = 2;
            grounded = true;
            jumping = false;
        }
        else
        {
            grounded = false;
        }

        RaycastHit2D wallJumpLeftRay1 = Physics2D.Raycast(pos + new Vector2(-0.5f, -0.5f), new Vector2(-1, 0), 0.05f);
        RaycastHit2D wallJumpLeftRay2 = Physics2D.Raycast(pos + new Vector2(-0.5f, 0.5f), new Vector2(-1, 0), 0.05f);
        Debug.DrawLine(transform.position + new Vector3(-0.5f, -0.5f), transform.position + new Vector3(-0.55f, -0.5f), wallJumpLeftRay1 ? Color.blue : Color.red, 0.05f, true);
        Debug.DrawLine(transform.position + new Vector3(-0.5f, 0.5f), transform.position + new Vector3(-0.55f, 0.5f), wallJumpLeftRay2 ? Color.blue : Color.red, 0.05f, true);

        if (!grounded && (wallJumpLeftRay1 || wallJumpLeftRay2) && spacePressed)
        {
            rb.AddForce(wallJumpForce, ForceMode2D.Impulse);
        }

        RaycastHit2D wallJumpRightRay1 = Physics2D.Raycast(pos + new Vector2(0.5f, -0.5f), new Vector2(1, 0), 0.05f);
        RaycastHit2D wallJumpRightRay2 = Physics2D.Raycast(pos + new Vector2(0.5f, 0.5f), new Vector2(1, 0), 0.05f);
        Debug.DrawLine(transform.position + new Vector3(0.5f, -0.5f), transform.position + new Vector3(0.55f, -0.5f), wallJumpRightRay1 ? Color.blue : Color.red, 0.05f, true);
        Debug.DrawLine(transform.position + new Vector3(0.5f, 0.5f), transform.position + new Vector3(0.55f, 0.5f), wallJumpRightRay2 ? Color.blue : Color.red, 0.05f, true);

        if (!grounded && (wallJumpRightRay1 || wallJumpRightRay2) && spacePressed)
        {
            rb.AddForce(new Vector2(-wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
        }

        Vector2 dragForce = DragForce();
        rb.velocity += new Vector2(Input.GetAxis("Horizontal") * (grounded ? accel : accel * airAccelMult) - dragForce.x - (grounded ? groundFriction * rb.velocity.x: 0), ((!grounded && !dashNoGravity) ? -gravity : 0) - dragForce.y);

        if ((Input.GetAxis("Vertical") > 0 || Input.GetKeyDown("space")) && (grounded || timeSinceGrounded > 0.0f) && !jumping)
        {
            rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
            jumping = true;
        }
        if (timeSinceGrounded > 0.0f)
        {
            timeSinceGrounded -= Time.deltaTime;
        }

        if (shiftPressed && canDash && (airDashes > 0 || grounded))
        {
            StartCoroutine(Dash());
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            sr.flipX = true;
        } 
        else if (Input.GetAxis("Horizontal") > 0)
        {
            sr.flipX = false;
        }

        print(String.Format("airDashes: {11}, canDash: {10}, grounded: {3}, vel(x: {0}, y: {1}), timeSinceGrounded: {4}, jumping: {5}, terminal_velocity: {8}, dragForce(x: {6}, y: {7}), Horizontal: {9}, Vertical: {2}", rb.velocity.x, rb.velocity.y, Input.GetAxis("Vertical"), grounded, timeSinceGrounded, jumping, dragForce.x, dragForce.y, terminal_velocity(), Input.GetAxis("Horizontal"), canDash, airDashes));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        //if (collision.gameObject.tag == "Ground") {
        //    float playerTop = transform.position.y + transform.position.y * transform.localScale.y / 2;
        //    float objectTop = collision.gameObject.transform.position.y + collision.gameObject.transform.position.y * collision.gameObject.transform.localScale.y / 2;
        //    if (playerTop - objectTop > 0) { 
        //        grounded = true;
        //        jumping = false;
        //        airDashes = 2;
        //    } else {;
        //        rb.velocity = new Vector2(0, rb.velocity.y);
        //    }
        //}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (collision.gameObject.tag == "Ground")
        //{
        //    timeSinceGrounded = coyoteTime;
        //    grounded = false;
        //}
    }

    private IEnumerator Dash()
    {
        if (grounded || airDashes > 0) { 
            if (!grounded) { airDashes--; }
            foreach(BoxCollider2D i in dashWalls)
            {
                i.enabled = false;
            }
            sr.sprite = dashingSprite;
            canDash = false;
            dashNoGravity = true;
            rb.velocity = new Vector2(dashDistance * horizontalLastHeldDirection, 0f);
            tr.emitting = true;
            yield return new WaitForSeconds(dashTime);
            foreach (BoxCollider2D i in dashWalls)
            {
                i.enabled = true;
            }
            sr.sprite = normalSprite;
            dashNoGravity = false;
            tr.emitting = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
        }
    }

    private void MiniJump()
    {
        rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
    }

    public void SetRespawnPoint(Vector2 position)
    {
        respawnPoint = position;
    }

    public void Die()
    {
        active = false;
        collider.enabled = false;
        sr.sprite = dyingSprite;
        MiniJump();
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.7f);
        transform.position = respawnPoint;
        active = true;
        collider.enabled = true;
        sr.sprite = normalSprite;
        MiniJump();
    }
}
