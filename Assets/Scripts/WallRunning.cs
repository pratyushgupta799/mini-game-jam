using System;
using UnityEditor.Searcher;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float wallRunForce;
    [SerializeField] private float maxWallRunTime;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float wallJumpForce;
    
    private float wallRunTimer;

    [Header("Input")] 
    private float horizontalInput;
    private float verticalInput;
    
    [Header("Detection")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private RaycastHit frontWallHit;
    private bool wallLeft;
    private bool wallRight;
    private bool wallFront;
    
    [Header("References")]
    [SerializeField] private Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    private bool wallRunning = false;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
        if (Input.GetKeyDown(pm.jumpKey) && wallRunning)
        {
            StopWallRun();
            pm.WallJump(wallJumpForce);
        }
    }

    private void FixedUpdate()
    {
        if (wallRunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
        wallFront = Physics.Raycast(transform.position, orientation.forward, out frontWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        if((wallLeft || wallRight || wallFront) && AboveGround())
        {
            if (!wallRunning)
            {
                StartWallRun();
            }
        }

        else
        {
            if (wallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        wallRunning = true;
        pm.moveSpeed = wallRunSpeed;
    }
    
    private void WallRunningMovement()
    {
        Debug.Log("Wall run");

        rb.useGravity = false;

        // Cancel vertical velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Determine wall normal
        Vector3 wallNormal = Vector3.zero;
        if (wallRight) wallNormal = rightWallHit.normal;
        else if (wallLeft) wallNormal = leftWallHit.normal;
        else if (wallFront) wallNormal = frontWallHit.normal;

        // Calculate forward direction along the wall
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        // Make sure we're moving in the correct direction
        if (Vector3.Dot(orientation.forward, wallForward) < 0)
        {
            wallForward = -wallForward;
        }

        // Move player along the wall
        //rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Stick to wall
        rb.AddForce(-wallNormal * 50f, ForceMode.Force); // You can tweak the 50f
    }


    private void StopWallRun()
    {
        Debug.Log("Wall run stopped");
        //rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
        wallRunning = false;
        rb.useGravity = true;
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     if (Collision.other.layer == "wall")
    //     {
    //         
    //     }
    // }
}
