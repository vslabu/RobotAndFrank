using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC_Controller : MonoBehaviour
{
	public CharacterController characterController;
	
	public float movementSpeed = 0.1f;
	public float turningTime = 1f;
	protected float turnSmoothVelocity;

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	protected void moveTowards(Vector3 target) //Moves NPC toward target
    {
		turnTowards(target);

		Vector3 movement = (target - vectorToPlaneVector(transform.position)).normalized;
		characterController.Move(movement * movementSpeed * Time.fixedDeltaTime);
    }

	protected void turnTowards(Vector3 lookAt)//Turns NPC towards target
	{
		float targetAngle = Mathf.Atan2(lookAt.x, lookAt.z) * Mathf.Rad2Deg;
		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turningTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}

	protected Vector3 vectorToPlaneVector(Vector3 vector) //Sets y value of vector to 0
	{
		vector.y = 0;
		return vector;
	}
}
