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

		move = new Vector3 (horzVel,fallVel,vertVel);
		move = transform.TransformVector (move);

		if (Grounded ()) 
		{
			fallVel = 0f;
			vertVel = Input.GetAxis ("Vertical") * groundAccel;
			horzVel = Input.GetAxis ("Horizontal") * groundAccel;

			if (Input.GetButton ("Jump")) 
				fallVel = jumpAccel;
		}

		if (!Grounded())
			fallVel += (Physics.gravity.y * Time.deltaTime);

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
		
}
