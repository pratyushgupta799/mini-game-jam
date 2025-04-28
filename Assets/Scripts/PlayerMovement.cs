// PlayerMovement
using System;
using UnityEditor.Searcher;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private AudioClip jumpSFX;
	private AudioSource audioSource;
	
    [Header("Assignables")]
    //Assignables
	public Transform playerCam;
	public Transform orientation;
	private Collider playerCollider;
	public Rigidbody rb;

    [Space(10)]

	public LayerMask whatIsGround;
	public LayerMask whatIsWallrunnable;

    [Header("MovementSettings")]
    //Movement Settings 
	public float sensitivity = 50f;
	public float moveSpeed = 4500f;
	public float walkSpeed = 20f;
	public float runSpeed = 10f;
	public bool grounded;
	public bool onWall;
	[SerializeField] private float moveForwardSpeed;
	[SerializeField] private float moveSideSpeed;

    //Private Floats
    private float wallRunGravity = 1f;
	private float maxSlopeAngle = 35f;
	private float wallRunCamRotation = 0f;
    private float slideSlowdown = 0.2f;
	private float actualWallRunRotation;
	private float wallRotationVel;
	private float desiredX;
	private float xRotation;
	private float sensMultiplier = 1f;
	private float jumpCooldown = 0.25f;
	[SerializeField]private float jumpForce = 550f;
	private float sideInput;
	private float forwardInput;
	private float vel;

    //Private bools
	private bool readyToJump;
	private bool jumping;
	private bool sprinting;
    private bool crouching;
	private bool wallRunning;
    private bool cancelling;
	private bool readyToWallrun = true;
    private bool airborne;
    private bool onGround;
	private bool surfing;
	private bool cancellingGrounded;
	private bool cancellingWall;
	private bool cancellingSurf;

    //Private Vector3's
	private Vector3 grapplePoint;
	private Vector3 normalVector;
	private Vector3 wallNormalVector;
	private Vector3 wallRunPos;
	private Vector3 previousLookdir;

	private bool isJumping = false;
	private bool canJumpFr = true;
	private float coyoteTimer;
	private float coyoteTime = 0.2f;
	
    //Private int
	private int nw;
    
    //Instance
	public static PlayerMovement Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		//DontDestroyOnLoad(gameObject);
		//rb.position = new Vector3(0f, 0f, 0f);
	}

	private void Start()
	{
		playerCollider = GetComponent<Collider>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		readyToJump = true;
		wallNormalVector = Vector3.up;
	}

	private void LateUpdate()
	{
        //For wallrunning
	    WallRunning();
	}

	private void FixedUpdate()
	{
        //For moving
		Movement();
	}

	private void Update()
	{
        //Input
		MyInput();
        //Looking around
		Look();
		PositionCheck();
		UpdateCoyoteTime();
	}

    //Player input
	private void MyInput()
	{
		sideInput = Input.GetAxisRaw("Horizontal");
		forwardInput = Input.GetAxisRaw("Vertical");
		jumping = Input.GetButton("Jump");
		crouching = Input.GetKey(KeyCode.LeftShift);
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			StartCrouch();
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			StopCrouch();
		}
	}

    //Scale player down
	private void StartCrouch()
	{
		float num = 400f;
		transform.localScale = new Vector3(1f, 0.5f, 1f);
		transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
		if (rb.linearVelocity.magnitude > 0.1f && grounded)
		{
			rb.AddForce(orientation.transform.forward * num);
		}
	}

    //Scale player to original size
	private void StopCrouch()
	{
		transform.localScale = new Vector3(1f, 1.5f, 1f);
		transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
	}

    //Moving around with WASD
	private void Movement()
	{
		rb.AddForce(Vector3.down * Time.deltaTime * 10f);
		Vector2 relativeToLook = FindVelRelativeToLook();
		float strafe = relativeToLook.x;
		float forward = relativeToLook.y;
		CounterMovement(sideInput, forwardInput, relativeToLook);
		if (readyToJump && jumping)
		{
			Jump();
		}
		float maxSpeed = walkSpeed;
		if (sprinting)
		{
			maxSpeed = runSpeed;
		}
		if (crouching && grounded && readyToJump)
		{
			rb.AddForce(Vector3.down * Time.deltaTime * 3000f);
			return;
		}
		if (forwardInput > 0f && strafe > maxSpeed)
		{
			forwardInput = 0f;
		}
		if (forwardInput < 0f && strafe < 0f - maxSpeed)
		{
			forwardInput = 0f;
		}
		if (sideInput > 0f && forward > maxSpeed)
		{
			sideInput = 0f;
		}
		if (sideInput < 0f && forward < 0f - maxSpeed)
		{
			sideInput = 0f;
		}
		float forwardSpeedFactor = 1f;
		float strafeSpeedFactor = 1f;
		if (!grounded)
		{
			forwardSpeedFactor = 0.5f;
			strafeSpeedFactor = 0.5f;
		}
		if (grounded && crouching)
		{
			strafeSpeedFactor = 0f;
		}
		if (wallRunning)
		{
			strafeSpeedFactor = 0.3f;
			forwardSpeedFactor = 0.3f;
		}
		if (surfing)
		{
			forwardSpeedFactor = 0.7f;
			strafeSpeedFactor = 0.3f;
		}
		
		
		rb.AddForce(orientation.transform.forward * forwardInput * moveForwardSpeed * Time.deltaTime * forwardSpeedFactor * 0.1f, ForceMode.VelocityChange);
		rb.AddForce(orientation.transform.right * sideInput * moveSideSpeed * 0.1f * Time.deltaTime * strafeSpeedFactor, ForceMode.VelocityChange);
	}

    //Ready to jump again
	private void ResetJump()
	{
		readyToJump = true;
	}
	
	private void UpdateCoyoteTime()
	{
		if (grounded || wallRunning)
		{
			coyoteTimer = coyoteTime; // Reset timer if grounded
		}
		else
		{
			coyoteTimer -= Time.deltaTime; // Count down otherwise
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (isJumping == true)
		{
			isJumping = false;
		}
	}

	//Player go fly
	private void Jump()
	{
        if ((coyoteTimer > 0) && !isJumping)
		{
			PlayJumpSound();
		    print("jumping");
		    isJumping = true;
		    SwapColor();
		    Vector3 velocity = rb.linearVelocity;
		    readyToJump = false;
		    rb.AddForce(Vector2.up * jumpForce * 1.5f);
		    rb.AddForce(normalVector * jumpForce * 0.5f);
		    if (rb.linearVelocity.y < 0.5f)
		    {
			    rb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
		    }
		    else if (rb.linearVelocity.y > 0f)
		    {
			    rb.linearVelocity = new Vector3(velocity.x, velocity.y / 2f, velocity.z);
		    }
		    if (wallRunning)
		    {
			    rb.AddForce(wallNormalVector * jumpForce * 3f);
		    }
		    Invoke("ResetJump", jumpCooldown);
		    if (wallRunning)
		    {
			    wallRunning = false;
		    }

		}
        
	}

	private void PlayJumpSound()
	{
		if (jumpSFX != null && audioSource != null)
		{
			audioSource.PlayOneShot(jumpSFX);
		}
	}

	private void SwapColor()
	{
		GameManager.Instance.SetActiveWalls();
	}

    //Looking around by using your mouse
	private void Look()
	{
		float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
		float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
		desiredX = playerCam.transform.localRotation.eulerAngles.y + mouseX;
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
		FindWallRunRotation();
		actualWallRunRotation = Mathf.SmoothDamp(actualWallRunRotation, wallRunCamRotation, ref wallRotationVel, 0.2f);
		playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, actualWallRunRotation);
		orientation.transform.localRotation = Quaternion.Euler(0f, desiredX, 0f);
	}

    //Make the player movement feel good 
	private void CounterMovement(float sideInput, float forwardInput, Vector2 relativeToLook)
	{
		// if (!grounded || jumping)
		// {
		// 	return;
		// }
		float reverseForce = 0.16f;
		float num2 = 0.01f;
		if (crouching)
		{
			rb.AddForce(moveSpeed * Time.deltaTime * -rb.linearVelocity.normalized * slideSlowdown);
			return;
		}
		if ((Math.Abs(relativeToLook.x) > num2 && Math.Abs(sideInput) < 0.05f) || (relativeToLook.x < 0f - num2 && sideInput > 0f) || (relativeToLook.x > num2 && sideInput < 0f))
		{
			rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * (0f - relativeToLook.x) * reverseForce);
		}
		if ((Math.Abs(relativeToLook.y) > num2 && Math.Abs(forwardInput) < 0.05f) || (relativeToLook.y < 0f - num2 && forwardInput > 0f) || (relativeToLook.y > num2 && forwardInput < 0f))
		{
			rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * (0f - relativeToLook.y) * reverseForce);
		}
		if (Mathf.Sqrt(Mathf.Pow(rb.linearVelocity.x, 2f) + Mathf.Pow(rb.linearVelocity.z, 2f)) > walkSpeed)
		{
			float upwardVelocity = rb.linearVelocity.y;
			Vector3 newVelocity = rb.linearVelocity.normalized * walkSpeed;
			rb.linearVelocity = new Vector3(newVelocity.x, upwardVelocity, newVelocity.z);
		}
	}

	public Vector2 FindVelRelativeToLook()
	{
		float current = orientation.transform.eulerAngles.y;
		float target = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * 57.29578f;
		float headingDifference = Mathf.DeltaAngle(current, target);
		float complementOfHeadingDifference = 90f - headingDifference;
		float magnitude = rb.linearVelocity.magnitude;
		return new Vector2(y: magnitude * Mathf.Cos(headingDifference * ((float)Math.PI / 180f)), x: magnitude * Mathf.Cos(complementOfHeadingDifference * ((float)Math.PI / 180f)));
	}

	private void FindWallRunRotation()
	{
		if (!wallRunning)
		{
			wallRunCamRotation = 0f;
			return;
		}
		_ = new Vector3(0f, playerCam.transform.rotation.y, 0f).normalized;
		new Vector3(0f, 0f, 1f);
		float num = 0f;
		float current = playerCam.transform.rotation.eulerAngles.y;
		num = Vector3.SignedAngle(new Vector3(0f, 0f, 1f), wallNormalVector, Vector3.up);
		float num2 = Mathf.DeltaAngle(current, num);
		wallRunCamRotation = (0f - num2 / 90f) * 15f;
		if (!readyToWallrun)
		{
			return;
		}
		if ((Mathf.Abs(wallRunCamRotation) < 4f && forwardInput > 0f && Math.Abs(sideInput) < 0.1f) || (Mathf.Abs(wallRunCamRotation) > 22f && forwardInput < 0f && Math.Abs(sideInput) < 0.1f))
		{
			if (!cancelling)
			{
				cancelling = true;
				CancelInvoke("CancelWallrun");
				Invoke("CancelWallrun", 0.2f);
			}
		}
		else
		{
			cancelling = false;
			CancelInvoke("CancelWallrun");
		}
	}

	private void CancelWallrun()
	{
		print("cancelled");
		wallRunCamRotation = 0f;
		Invoke("GetReadyToWallrun", 0.1f);
		rb.AddForce(wallNormalVector * 600f);
		readyToWallrun = false;
	}

	private void GetReadyToWallrun()
	{
		readyToWallrun = true;
	}

	private void WallRunning()
	{
		if (wallRunning)
		{
			// Check if player is facing the wall
			float dot = Vector3.Dot(orientation.forward, wallNormalVector);

			if (dot < 0.5f) // Adjust 0.5 based on how strict you want it
			{
				// Only push into wall if NOT directly facing it
				rb.AddForce(-wallNormalVector * Time.deltaTime * moveSpeed);
			}

			// Optional gravity effect while wallrunning (you had it commented)
			rb.AddForce(Vector3.up * Time.deltaTime * rb.mass * 100f * wallRunGravity);
		}
	}

	private bool IsFloor(Vector3 v)
	{
		return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
	}

	private bool IsSurf(Vector3 v)
	{
		float num = Vector3.Angle(Vector3.up, v);
		if (num < 89f)
		{
			return num > maxSlopeAngle;
		}
		return false;
	}

	private bool IsWall(Vector3 v)
	{
		return Math.Abs(90f - Vector3.Angle(Vector3.up, v)) < 0.1f;
	}

	private bool IsRoof(Vector3 v)
	{
		return v.y == -1f;
	}

	private void StartWallRun(Vector3 normal)
	{
		if (!grounded && readyToWallrun)
		{
			wallNormalVector = normal;
			float wallRunUpImpulse = 20f;
			if (!wallRunning)
			{
				rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
				rb.AddForce(Vector3.up * wallRunUpImpulse, ForceMode.Impulse);
			}
			wallRunning = true;
		}
	}

	private void OnCollisionStay(Collision other)
	{
		int layer = other.gameObject.layer;
		if ((int)whatIsGround != ((int)whatIsGround | (1 << layer)))
		{
			return;
		}
		for (int i = 0; i < other.contactCount; i++)
		{
			Vector3 normal = other.contacts[i].normal;
			if (IsFloor(normal))
			{
				if (wallRunning)
				{
					wallRunning = false;
				}
				grounded = true;
				normalVector = normal;
				cancellingGrounded = false;
				CancelInvoke("StopGrounded");
			}
			if (IsWall(normal) && layer == LayerMask.NameToLayer("Ground"))
			{
				StartWallRun(normal);
				onWall = true;
				cancellingWall = false;
				CancelInvoke("StopWall");
			}
			if (IsSurf(normal))
			{
				surfing = true;
				cancellingSurf = false;
				CancelInvoke("StopSurf");
			}
			IsRoof(normal);
		}
		float num = 3f;
		if (!cancellingGrounded)
		{
			cancellingGrounded = true;
			Invoke("StopGrounded", Time.deltaTime * num);
		}
		if (!cancellingWall)
		{
			cancellingWall = true;
			Invoke("StopWall", Time.deltaTime * num);
		}
		if (!cancellingSurf)
		{
			cancellingSurf = true;
			Invoke("StopSurf", Time.deltaTime * num);
		}
	}

	private void StopGrounded()
	{
		grounded = false;
	}

	private void StopWall()
	{
		onWall = false;
		wallRunning = false;
	}

	private void StopSurf()
	{
		surfing = false;
	}

	public Vector3 GetVelocity()
	{
		return rb.linearVelocity;
	}

	public float GetFallSpeed()
	{
		return rb.linearVelocity.y;
	}

	public Collider GetPlayerCollider()
	{
		return playerCollider;
	}

	public Transform GetPlayerCamTransform()
	{
		return playerCam.transform;
	}

	public bool IsCrouching()
	{
		return crouching;
	}

	public Rigidbody GetRb()
	{
		return rb;
	}

	private void PositionCheck()
	{
		if (transform.position.y < -40f)
		{
			GameManager.Instance.ReloadCurrentLevel();
		}
	}
}
