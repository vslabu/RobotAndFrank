using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NPC_Controller : MonoBehaviour
{
	protected CharacterController characterController;
	protected GameObject player;

	//The current behavior coroutine
	protected Coroutine currentBehaviorCoroutine;

	[Header("Movement Variables")]
	[SerializeField]
	protected float movementSpeed = 0.1f;
	[SerializeField]
	protected float turningTime = 1f;
	float turnSmoothVelocity;
	protected float epsilon = 0.01f;
	[SerializeField]
	protected float turningSpeed = 10f;
	[SerializeField]
	[Tooltip("The Distance, that determines wether NPC is touching another object.")]
	protected float touchingDistance = 1f;

	[Header("Hearing Variables")]
	[SerializeField]
	protected float hearingRadius = 20f;
	[SerializeField]
	public LayerMask hearingObstacleMask;

	protected void InitNPCMovement()
	{
		//Init Controller
		characterController = GetComponent<CharacterController>();

		//Init Player
		player = GameObject.FindGameObjectWithTag("Player");
	}

	#region coroutines
	//Stops the old behavior coroutine and starts the new one
	protected void ChangeCurrentCoroutine(IEnumerator coroutine)
	{
		if(currentBehaviorCoroutine != null)
		{
			StopCoroutine(currentBehaviorCoroutine);
		}

		currentBehaviorCoroutine = StartCoroutine(coroutine);
	}

	//Stops current coroutine
	protected void StopCurrentCoroutine()
	{
		if (currentBehaviorCoroutine != null)
		{
			StopCoroutine(currentBehaviorCoroutine);
		}
	}
	#endregion

	protected void MoveTowards(Vector3 target) //Moves NPC toward target
    {
		if(Vector3.Distance(target, transform.position) > movementSpeed * Time.deltaTime)
		{
			//target is not near
			Vector3 offset = target - transform.position;

			offset = offset.normalized * movementSpeed;
			characterController.SimpleMove(offset);
		} else
		{
			//target is near
			transform.position = target;
		}

		
	}

	#region Turning
	protected void TurnTowardsSmooth(Vector3 lookAt)//Turns NPC towards target
	{
		float targetAngle = Mathf.Atan2(lookAt.x, lookAt.z) * Mathf.Rad2Deg;
		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turningTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}

	protected void TurnTowardsSmooth(Vector3 lookAt, bool worldPosition)//Turns NPC towards target, true means that the position is given in world coordinates
	{
		if (worldPosition)
		{
			lookAt -= transform.position;
		}

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

	#endregion

	#region booleans
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
		return IsAlmostEqual(v1, v2, 0.1f);
	}

	//returns true, when two points have almost equal positions on the plane 
	protected bool IsAbove(Vector3 v1, Vector3 v2, float e)
	{
		v1.y = 0;
		v2.y = 0;
		return IsAlmostEqual(v1, v2, e);
	}

	protected bool LookingAt(Vector3 pos)
	{
		Vector3 lookingDirection = (pos - transform.position).normalized;
		return IsAbove(lookingDirection, transform.forward, 0.01f);
	}

	protected bool IsTouching(GameObject obj)
	{
		return Vector3.Distance(obj.transform.position, transform.position) < touchingDistance;
	}
	#endregion

	protected float GetPathLength(NavMeshPath path)
	{
		float lng = 0.0f;

		if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
		{
			for (int i = 1; i < path.corners.Length; ++i)
			{
				lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
			}
		}

		return lng;
	}
}
