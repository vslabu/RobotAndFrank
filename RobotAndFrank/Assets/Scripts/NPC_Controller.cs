using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC_Controller : MonoBehaviour
{
	protected CharacterController characterController;
	
	public float movementSpeed = 0.1f;
	public float turningTime = 1f;
	protected float turnSmoothVelocity;

	protected float epsilon = 0.5f;

	protected void InitSuper()
	{
		characterController = GetComponent<CharacterController>();
	}

	protected void MoveTowards(Vector3 target) //Moves NPC toward target
    {
		Vector3 offset = target - transform.position;

		TurnTowards(offset);

		offset = offset.normalized * movementSpeed;
		characterController.Move(offset * Time.deltaTime);
	}

	protected void TurnTowards(Vector3 lookAt)//Turns NPC towards target
	{
		float targetAngle = Mathf.Atan2(lookAt.x, lookAt.z) * Mathf.Rad2Deg;
		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turningTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}
}
