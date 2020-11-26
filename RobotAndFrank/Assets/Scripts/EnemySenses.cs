using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemySenses : NPC_Controller
{
	protected Frank_Controller frank;

	[Header("FoV Variables")]
	public float viewRadius = 10f;
	[Range(0,360)]
	public float viewAngle;
	[SerializeField]
	protected LayerMask visionObstacleMask;


	protected enum IdleBehavior
	{
		Stand,
		Turn,
		WalkOnPath
	}
	[Header("Idle Behavior")]
	[SerializeField]
	protected IdleBehavior idleBehavior = IdleBehavior.Stand;
	protected Vector3 startPosition;
	protected Vector3 startForwardDirection;

	[Space(10)]
	#region Turn Behavior

	[Range(0, 360)]
	public float turnAngle = 0;
	[HideInInspector]
	public Vector3 leftTurnBorder, rightTurnBorder;
	[SerializeField]
	protected float waitTimeTurn = 0.3f;

	#endregion
	[Space(10)]
	#region WalkOnPath Behavior

	public Transform pathHolder;
	protected Vector3[] waypoints;
	[SerializeField]
	protected float waitTimePath = 0.3f;

	#endregion


	protected void InitSenses()
    {
		InitNPCMovement();
		GameObject frankObject = GameObject.FindGameObjectWithTag("Frank");
		frank = frankObject.GetComponent<Frank_Controller>();

		//Init startPosition
		startPosition = transform.position;
		startForwardDirection = transform.forward;

		//Init vectors used in Turning behavior
		leftTurnBorder = transform.forward;
		rightTurnBorder = DirFromAngle(turnAngle, false);

		//Init path used in MoveOnPath Behavior
		if (pathHolder != null)
		{
			waypoints = new Vector3[pathHolder.childCount];
			for (int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i] = pathHolder.GetChild(i).position;
			}
		}

	}

	#region Idle Behaviors

	protected IEnumerator Stand()
	{
		//TODO: Implement Stand behavior
		yield return null;
	}

	protected IEnumerator Turn()
	{
		while (true)
		{
			//Turn right
			while (!IsAlmostEqual(transform.forward, rightTurnBorder))
			{
				TurnRightTowards(rightTurnBorder);
				yield return null;
			}

			yield return new WaitForSeconds(waitTimeTurn);

			//Turn left
			while (!IsAlmostEqual(transform.forward, leftTurnBorder))
			{
				TurnLeftTowards(leftTurnBorder);
				yield return null;
			}

			yield return new WaitForSeconds(waitTimeTurn);
		}
	}

	public bool HasTurnBehavior()
	{
		return idleBehavior == IdleBehavior.Turn;
	}

	protected IEnumerator WalkOnPath(int startWaypoint)
	{
		int targetWaypointIndex = startWaypoint;

		while (true)
		{
			//turn towards next waypoint
			while (!LookingAt(waypoints[targetWaypointIndex]))
			{
				TurnTowardsSmooth(waypoints[targetWaypointIndex], true);
				yield return null;
			}

			//move towards next waypoint
			while (!IsAbove(transform.position, waypoints[targetWaypointIndex]))
			{
				MoveTowards(waypoints[targetWaypointIndex]);
				yield return null;
			}

			//set new target
			targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
			yield return new WaitForSeconds(waitTimePath);
		}
	}

	protected int FindNearestWaypoint()
	{
		if(pathHolder == null)
		{
			Debug.LogError("No path was found!");
			return 0;
		}
		NavMeshPath path = new NavMeshPath();
		int indexOfNearestWaypoint = 0;
		float shortestPathLength = Mathf.Infinity;

		for (int i = 0; i < waypoints.Length; i++)
		{
			agent.CalculatePath(waypoints[i], path);
			float pathLength = GetPathLength(path);

			if (pathLength < shortestPathLength)
			{
				shortestPathLength = pathLength;
				indexOfNearestWaypoint = i;
			}
		}

		return indexOfNearestWaypoint;
	}

	#endregion


	protected bool CanSeeFrank()
	{
		if(!CanSeePosition(frank.transform.position, viewRadius, visionObstacleMask)){
			//Frank is either too far away or hidden by an obstacle
			return false;
		}

		Vector3 directionToFrank = (frank.transform.position - transform.position).normalized;
		if(Vector3.Angle(transform.forward, directionToFrank) < viewAngle / 2)
		{
			//Frank is in the viewangle of the guard
			return true;
		}

		return false;
	}

	public Vector3 DirFromAngle (float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}


	private void OnDrawGizmos()
	{
		//Draw Path, if Idle Behavior = MoveOnPath
		if (idleBehavior == IdleBehavior.WalkOnPath)
		{
			Gizmos.color = Color.green;
			Vector3 startPosition = pathHolder.GetChild(0).position;
			Vector3 previosPosition = startPosition;
			foreach (Transform waypoint in pathHolder)
			{
				Gizmos.DrawSphere(waypoint.position, .2f);
				Gizmos.DrawLine(previosPosition, waypoint.position);
				previosPosition = waypoint.position;
			}
			Gizmos.DrawLine(previosPosition, startPosition);
		}
	}
}
