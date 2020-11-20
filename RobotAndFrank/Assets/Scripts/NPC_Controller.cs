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

	protected void InitNPCMovement()
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


	//Checks if there is a wall between the NPC and the position and if they are not to far appart.
	//layerMask is the Mask, with all the walls the NPC cant see through
	protected bool CanSeePosition(Vector3 pos, float sightDistance, LayerMask layerMask)
	{
		//Check if the distance is to far
		float distanceToPos = Vector3.Distance(transform.position, pos);
		if(distanceToPos > sightDistance)
		{
			return false;
		}

		//Check if there is a wall inbetween
		if (Physics.Linecast(pos, transform.position, layerMask))
		{
			return false;
		}
		return true;
	}
}
