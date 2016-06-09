using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private Rigidbody rigid;
	private Camera playerCam;

	[SerializeField] float mouseXSens = 2f;
	[SerializeField] float mouseYSens = 2f;
	[SerializeField] float groundSpeedCap = 7;

	[SerializeField] private float groundAccel = 10f;
	[SerializeField] private float groundFriction = 2f;
	[SerializeField] private Vector3 velocity;
	[SerializeField] private Vector3 move;
	[SerializeField] private bool forwardInput;
	[SerializeField] private bool strafeInput;


	void Awake () 
	{

		rigid = GetComponent<Rigidbody> ();
		playerCam = Camera.main;
	
	}

	void Update () 
	{
		

		velocity.y += (Physics.gravity.y * Time.deltaTime);

		if (Grounded()) 
		{
			velocity.y = 0;

			//TODO: condense input IFs into one vector
			if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0) 
			{
				velocity.z += (Input.GetAxis("Vertical") * (groundAccel)) * (Time.deltaTime * 2) ;
				forwardInput = true;
			}
			else 
			{
				forwardInput = false;
			}

			if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0) 
			{
				velocity.x -= (Input.GetAxis("Horizontal") * (groundAccel)) * (Time.deltaTime * 2) ;
				print (velocity.x);
				strafeInput = true;
			}
			else 
			{
				strafeInput = false;
			}

			//cap magnitude of that condensed vector to the run spepd cap
			/*if (velocity.magnitude > groundSpeedCap) 
			{
				Vector3.ClampMagnitude (velocity, groundSpeedCap);
			}*/
		}

		if (Input.GetAxis ("Mouse X") > 0 || Input.GetAxis ("Mouse X") < 0 ) 
		{
			float playerXRot = Input.GetAxis ("Mouse X") * mouseXSens;
			transform.Rotate (Vector3.up, playerXRot);
		}

		if (!forwardInput) 
		{
			if (velocity.z > 0)
				velocity.z -= groundFriction;
			if (velocity.z < 0)
				velocity.z += groundFriction;
			
			if (velocity.z > -0.2f && velocity.z < 0.2f)
				velocity.z = 0f;
		}
			
		//oreient that condensed vector to match players local forward
		move = playerCam.transform.forward;
		move.x *= velocity.x;
		move.y = velocity.y;
		move.z *= velocity.z;
			
	
	}

	void FixedUpdate ()
	{
		Grounded ();

		rigid.velocity = move;
	}
	bool Grounded ()
	{
		Ray downRay = new Ray();
		downRay.origin = this.gameObject.transform.position;
		downRay.direction = Vector3.down;
		RaycastHit hit;
		Physics.Raycast (downRay,out hit,1.1f);

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
		
}
