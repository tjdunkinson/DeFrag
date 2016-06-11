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
	[SerializeField] private float gravity;
	[SerializeField] private float sailAccel = 8f;
	[SerializeField] private float frictionDelay = 0.5f;
	[SerializeField] private float delayTimer;
	[SerializeField] private Vector3 move;



	void Awake () 
	{

		rigid = GetComponent<Rigidbody> ();
		playerCam = Camera.main;
		Cursor.lockState = CursorLockMode.Locked;
	
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

		//move = new Vector3 (horzVel,fallVel,vertVel);


		if (Grounded ()) 
		{
			move.y = 0f;

			delayTimer -= Time.deltaTime;

			if (delayTimer < 0) 
			{
				delayTimer = 0;

				move.z =Input.GetAxis ("Vertical") * groundAccel;
				move.x = Input.GetAxis ("Horizontal") * groundAccel;
				move = transform.TransformDirection (move);
			} 
			else 
			{
				//speedbosting, not working?
				float speed = 0;
				Vector2 magCheck = new Vector2 (rigid.velocity.x, rigid.velocity.z);
				if (magCheck.magnitude > groundAccel)
				{
					speed = (magCheck.magnitude - groundAccel) /2;
					//apply speedBoost to move
				}

				Vector3 speedBoost = move;
				speedBoost = transform.InverseTransformDirection (speedBoost);
				speedBoost.z = ((Input.GetAxis ("Vertical") * groundAccel) + speed);
				speedBoost.x = ((Input.GetAxis ("Horizontal") * groundAccel) + speed);
				speedBoost = transform.TransformDirection (speedBoost);
				move.x = speedBoost.x;
				move.z = speedBoost.z;

				move = transform.TransformDirection (move);
			}

			if (Input.GetButton ("Jump")) 
			{
				Vector2 magCheck = new Vector2 (rigid.velocity.x, rigid.velocity.z);
				if (magCheck.magnitude > groundAccel)
				{
					float speedBoost = magCheck.magnitude - groundAccel;
					//apply speedBoost to move

				}

				move.y = jumpAccel;


			}


		}

		if (!Grounded ()) 
		{
			Vector3 airAccel = move;
			airAccel = transform.InverseTransformDirection (airAccel);
			airAccel.x += (Input.GetAxis ("Horizontal") * sailAccel) * Time.deltaTime;
			airAccel = transform.TransformDirection (airAccel);
			move.x = airAccel.x;
			move.z = airAccel.z;

			move.y += (gravity * Time.deltaTime);
			delayTimer = frictionDelay;
		}

	}

	void FixedUpdate ()
	{
		Grounded ();
		rigid.velocity = move;
		print (rigid.velocity.magnitude);

	}
	bool Grounded ()
	{
		Ray downRay = new Ray();
		downRay.origin = this.gameObject.transform.position;
		downRay.direction = Vector3.down;
		RaycastHit hit;
		Physics.SphereCast (downRay, 0.5f, out hit, 1f);

		if (hit.collider) 
		{
			if (hit.collider.gameObject.layer == 9) 
			{
				/*Vector3 offest = hit.point;
				offest.y = offest.y + 2f;
				transform.position = offest;*/
				return true;
			}
			else
				return false;
		} 
		else
			return false;

	}
		
}
