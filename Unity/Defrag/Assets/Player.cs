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
	[SerializeField] private Vector3 velocityMod;


	void Awake () 
	{

		rigid = GetComponent<Rigidbody> ();
		playerCam = Camera.main;
	
	}

	void Update () 
	{

		velocityMod.y += (Physics.gravity.y * Time.deltaTime);

		if (DownRay()) 
		{
			velocityMod.y = 0;
			velocityMod.z -= groundFriction;
			if (Input.GetKey(KeyCode.W))
			{
				velocityMod.z += groundAccel * (Time.deltaTime * 2);
			}
		}

		if (Input.GetAxis ("Mouse X") > 0 || Input.GetAxis ("Mouse X") < 0 ) 
		{
			float playerXRot = Input.GetAxis ("Mouse X") * mouseXSens;
			transform.Rotate (Vector3.up, playerXRot);
		}
			
		if (velocityMod.z <= 0) 
		{
			velocityMod.z = 0;
		}
		if (velocityMod.z >= groundSpeedCap) 
		{
			velocityMod.z = groundSpeedCap;
		}
			

	}

	void FixedUpdate ()
	{
		DownRay ();

		rigid.velocity = velocityMod;
	}
	bool DownRay ()
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
