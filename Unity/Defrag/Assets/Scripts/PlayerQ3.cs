using UnityEngine;
using System.Collections;

public class PlayerQ3 : MonoBehaviour {

	private Rigidbody rigid;
	private Camera playerCam;

	//Quake Engine Variables
	public float friction;
	public float ground_accelerate;
	public float max_velocity_ground;
	public float air_accelerate;
	public float max_velocity_air;
	public float gravity;


	[SerializeField] float mouseXSens = 2f;
	[SerializeField] float mouseYSens = 2f;
	[SerializeField] float groundSpeedCap = 7;

	[SerializeField] private float groundAccel = 10f;
	[SerializeField] private float jumpAccel = 10f;
	[SerializeField] private float groundFriction = 2f;
	[SerializeField] private float fallVel;
	[SerializeField] private float vertVel;
	[SerializeField] private float horzVel;
	[SerializeField] private Vector3 move;


	void Awake () 
	{

		rigid = GetComponent<Rigidbody> ();
		playerCam = Camera.main;
		Cursor.lockState = CursorLockMode.Confined;
	
	}

	void Update () 
	{
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
		vertVel = Input.GetAxis ("Vertical");
		horzVel = Input.GetAxis ("Horizontal") ;
		move = new Vector3 (horzVel,fallVel,vertVel);
		move = transform.TransformVector (move);

		if (Grounded ()) 
		{
			fallVel = 0f;

			if (Input.GetButton ("Jump")) 
				fallVel = jumpAccel;

			MoveGround (move,rigid.velocity);
		}

		if (!Grounded ()) 
		{
			MoveAir (move,rigid.velocity);
		}
			


	}

	void FixedUpdate ()
	{
		Grounded ();
		//rigid.velocity = ;

	}
	bool Grounded ()
	{
		Ray downRay = new Ray();
		downRay.origin = this.gameObject.transform.position;
		downRay.direction = Vector3.down;
		RaycastHit hit;
		Physics.SphereCast (downRay, 0.5f, out hit, 1.1f);

		if (hit.collider) 
		{
			if (hit.collider.gameObject.layer == 9)
				return true;
			else
				return false;
		} 
		else
			return false;

		//OnDrawGizmos (downRay);
	}
	/*void OnDrawGizmos (Ray ray)
	{
		Gizmos.DrawRay (ray);
		Gizmos.DrawSphere ((ray.direction.magnitude + 1.1f), 0.5f);
	}*/

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
