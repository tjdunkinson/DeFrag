using UnityEngine;
using System.Collections;

public class PlayerQ3 : MonoBehaviour {

	private Rigidbody rigid;
	private CharacterController controller;
	private Camera playerCam;

	//Quake Engine Variables
	public float friction = 8;
	public float ground_accelerate = 50;
	public float max_velocity_ground = 4;
	public float air_accelerate = 100;
	public float max_velocity_air = 2;
	public float gravity = 800;

	private Vector3 wishDir;
	[SerializeField] float mouseXSens = 2f;
	[SerializeField] float mouseYSens = 2f;
	//[SerializeField] float groundSpeedCap = 7;

	//[SerializeField] private float groundAccel = 10f;
	[SerializeField] private float jumpAccel = 10f;
	//[SerializeField] private float groundFriction = 2f;
	[SerializeField] private float fallVel;
	//[SerializeField] private float vertVel;
	//[SerializeField] private float horzVel;
	//[SerializeField] private Vector3 move;


	void Awake () 
	{
		rigid = GetComponent<Rigidbody> ();
		//controller = GetComponent<CharacterController> ();
		playerCam = Camera.main;
		Cursor.lockState = CursorLockMode.Locked;
	
	}

	void Update () 
	{
		Grounded ();
		fallVel -= gravity * Time.deltaTime;

		if (Input.GetAxis ("Mouse X") > 0 || Input.GetAxis ("Mouse X") < 0 ) 
		{
			float playerXRot = Input.GetAxis ("Mouse X") * mouseXSens;
			transform.Rotate (Vector3.up, playerXRot);
		}
		if (Input.GetAxis ("Mouse Y") > 0 || Input.GetAxis ("Mouse Y") < 0 ) 
		{
			float playerYRot = Input.GetAxis ("Mouse Y") * mouseYSens;
			playerCam.transform.Rotate (Vector3.left, playerYRot);
		}


		wishDir = new Vector3 (Input.GetAxis ("Horizontal"),0,Input.GetAxis ("Vertical"));
		wishDir = transform.TransformDirection (wishDir);
		wishDir = wishDir.normalized;
	
		if (Grounded()) 
		{
			fallVel = 0;
			if (Input.GetButton ("Jump")) 
			{
				fallVel = jumpAccel;


			}
		}

		Vector2 magCheck = new Vector2 (rigid.velocity.x, rigid.velocity.z);
		print (magCheck.magnitude);
	}

	void FixedUpdate ()
	{
		if (Grounded ()) 
		{
			rigid.velocity = MoveGround (wishDir, rigid.velocity);
			//controller.SimpleMove(MoveGround (wishDir, controller.velocity));
		} 
		else 
		{
			rigid.velocity = MoveAir (wishDir, rigid.velocity);
			//controller.SimpleMove(MoveAir (wishDir, controller.velocity));
		}
		//print (rigid.velocity.magnitude);

		Vector3 vel = rigid.velocity;
		vel.y = fallVel;
		rigid.velocity = vel;
	}
	bool Grounded ()
	{
		Ray downRay = new Ray();
		downRay.origin = this.gameObject.transform.position;
		downRay.direction = Vector3.down;
		RaycastHit hit;
		Physics.SphereCast (downRay, 1f, out hit, 1f);

		if (hit.collider) 
		{
			if (hit.collider.gameObject.layer == 9)
				return true;
			else
				return false;
		} 
		else
			return false;

	}

	// accelDir: normalized direction that the player has requested to move (taking into account the movement keys and look direction)
	// prevVelocity: The current velocity of the player, before any additional calculations
	// accelerate: The server-defined player acceleration value
	// max_velocity: The server-defined maximum player velocity (this is not strictly adhered to due to strafejumping)
	private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
	{
		float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
		float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment

		// If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
		if(projVel + accelVel > max_velocity)
			accelVel = max_velocity - projVel;

		return prevVelocity + accelDir * accelVel;
	}

	private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
	{
		// Apply Friction
		float speed = prevVelocity.magnitude;
		if (speed != 0) // To avoid divide by zero errors
		{
			float drop = speed * friction * Time.fixedDeltaTime;
			prevVelocity *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction.
		}

		// ground_accelerate and max_velocity_ground are server-defined movement variables
		return Accelerate(accelDir, prevVelocity, ground_accelerate, max_velocity_ground);
	}

	private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
	{
		// air_accelerate and max_velocity_air are server-defined movement variables
		return Accelerate(accelDir, prevVelocity, air_accelerate, max_velocity_air);
	}
		
}
