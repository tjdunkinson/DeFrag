using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private Rigidbody rigid;
	private Camera playerCam;

	[SerializeField] float mouseXSens = 2f;
	[SerializeField] float mouseYSens = 2f;
	[SerializeField] float groundSpeedCap = 7;

	[SerializeField] private float groundAccel = 10f;
	[SerializeField] private float jumpAccel = 10f;
	[SerializeField] private float groundFriction = 2f;
	[SerializeField] private Vector3 velocity;
	[SerializeField] private float fallVel;
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

		move = new Vector3 (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical"));
		move = move.normalized;
		print (move);

		if (move.x != 0) 
		{
			move.x *= (groundAccel * Time.deltaTime);
		}
		if (move.z != 0) 
		{
			move.z *= (groundAccel * Time.deltaTime);
		}
		move = transform.TransformVector (move);
		//Gravity set to 50
		//fallVel += (Physics.gravity.y * Time.deltaTime);

		if (Grounded ()) 
		{
			fallVel = 0;
		

			//TODO: condense input IFs into one vector



			if (Input.GetButton ("Jump")) 
			{
				fallVel = jumpAccel;
			}



		} 
		else 
		{
			fallVel += (Physics.gravity.y * Time.deltaTime);
		}

		if (Input.GetAxis ("Mouse X") > 0 || Input.GetAxis ("Mouse X") < 0 ) 
		{
			float playerXRot = Input.GetAxis ("Mouse X") * mouseXSens;
			transform.Rotate (Vector3.up, playerXRot);
		}

		move.y = fallVel;


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
		Physics.Raycast (downRay,out hit,2.1f);

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
