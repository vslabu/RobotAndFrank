using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC_Controller : MonoBehaviour
{
	protected CharacterController characterController;

	[Header("Movement Variables")]
	[SerializeField]
	protected float movementSpeed = 0.1f;
	[SerializeField]
	protected float turningTime = 1f;
	float turnSmoothVelocity;
	protected float epsilon = 0.01f;
	[SerializeField]
	protected float turningSpeed = 10f;

	protected void InitNPCMovement()
	{
		characterController = GetComponent<CharacterController>();
	}

	protected void MoveTowards(Vector3 target) //Moves NPC toward target
    {
		Vector3 offset = target - transform.position;

		TurnTowardsSmooth(offset);

		offset = offset.normalized * movementSpeed;
		characterController.SimpleMove(offset);
	}

	protected void TurnTowardsSmooth(Vector3 lookAt)//Turns NPC towards target
	{
		float targetAngle = Mathf.Atan2(lookAt.x, lookAt.z) * Mathf.Rad2Deg;
		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turningTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}

	protected void TurnRightTowards(Vector3 lookAt)
	{
		float rotationPerUpdate = turningSpeed * Time.deltaTime;
		float targetAngle = Vector3.Angle(transform.forward, lookAt);
		float rotation = Mathf.Min(rotationPerUpdate, targetAngle);
		transform.Rotate(Vector3.up, rotation);
	}

	protected void TurnLeftTowards(Vector3 lookAt)
	{
		float rotationPerUpdate = - turningSpeed * Time.deltaTime;
		float targetAngle = Vector3.Angle(transform.forward, lookAt);
		float rotation = Mathf.Min(rotationPerUpdate, targetAngle);
		transform.Rotate(Vector3.up, rotation);
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

	//Returns true, when two vector are almost equal
	protected bool IsAlmostEqual(Vector3 v1, Vector3 v2)
	{
		if(Vector3.Distance(v1,v2) < epsilon)
		{
			return true;
		}
		return false;
	}

	protected bool IsAlmostEqual(Vector3 v1, Vector3 v2, float e)
	{
		if (Vector3.Distance(v1, v2) < e)
		{
			return true;
		}
		return false;
	}

	//returns true, when two points have almost equal positions on the plane 
	protected bool IsAbove(Vector3 v1, Vector3 v2)
	{
		v1.y = 0;
		v2.y = 0;
		return IsAlmostEqual(v1, v2);
	}
}
