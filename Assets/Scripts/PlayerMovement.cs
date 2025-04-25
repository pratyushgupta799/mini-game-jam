using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump = true;
    
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    
    [Header("Ground Check")] 
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;

    [SerializeField] private Transform orientation;
    
    private float horizontalInput;
    private float verticalInput;
    
    private Vector3 moveDirection;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        MyInput();

        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
        }
    }

    public void Jump()
    {
        //rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void WallJump(float wallJumpForce)
    {
        Debug.Log("Wall Jump");
        //rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
        rb.AddForce((orientation.transform.forward * wallJumpForce) + (transform.up * (jumpForce * 10f)), ForceMode.Force);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
