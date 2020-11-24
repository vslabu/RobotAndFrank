using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NormalGuard : EnemySenses
{
	#region Behavior

	enum Behavior
	{
		Idle, //Normal behavior, see enum IdleBehavior
		Suspicious, //Hears something, checks out the source
		DetectedFrank //Seeing Frank and going to him
	}

	enum IdleBehavior
	{
		Stand,
		Turn,
		WalkOnPath
	}

	[Header("Behavior")]
	[SerializeField]
	IdleBehavior idleBehavior = IdleBehavior.Stand;
	Behavior currentBehavior = Behavior.Idle;
	Vector3 startPosition;
	Vector3 startForwardDirection;
	[Space(10)]
	#region Turn Behavior

	[Range(0,360)]
	public float turnAngle = 0; //Only used if idleBehavior = Turn
	[HideInInspector]
	public Vector3 leftTurnBorder, rightTurnBorder;
	[SerializeField]
	float waitTimeTurn = 0.3f;

	#endregion
	[Space(10)]
	#region WalkOnPath Behavior

	public Transform pathHolder;
	Vector3[] waypoints;
	[SerializeField]
	float waitTimePath = 0.3f;

	#endregion

	#endregion

	NavMeshAgent agent;

	// Start is called before the first frame update
	void Start()
    {
		InitSenses();

		//Init startPosition
		startPosition = transform.position;
		startForwardDirection = transform.forward;

		//Init NavMeshAgent
		agent = GetComponent<NavMeshAgent>();
		agent.enabled = false;

		//Init vectors used in Turning behavior
		leftTurnBorder = transform.forward;
		rightTurnBorder = DirFromAngle(turnAngle, false);

		//Init path used in MoveOnPath Behavior
		if(pathHolder != null)
		{
			waypoints = new Vector3[pathHolder.childCount];
			for (int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i] = pathHolder.GetChild(i).position;
			}
		}

		Idle(0);
	}

	#region Behaviors
	private void Update()
	{
		if (CanSeeFrank() && currentBehavior != Behavior.DetectedFrank)
		{
			ChangeCurrentCoroutine(DetectedFrank());
		}
	}

	#region Idle Behaviors

	void Idle(int waypointIndex)
	{
		currentBehavior = Behavior.Idle;
		switch (idleBehavior)
		{
			case IdleBehavior.Stand:
				ChangeCurrentCoroutine(Stand());
				break;
			case IdleBehavior.Turn:
				ChangeCurrentCoroutine(Turn());
				break;
			case IdleBehavior.WalkOnPath:
				ChangeCurrentCoroutine(WalkOnPath(waypointIndex));
				break;
		}
	}

	IEnumerator Stand()
	{
		//TODO: Implement Stand behavior
		yield return null;
	}

	IEnumerator Turn()
	{
		while (true)
		{
			//Turn right
			while(!IsAlmostEqual(transform.forward, rightTurnBorder))
			{
				TurnRightTowards(rightTurnBorder);
				yield return null;
			}

			yield return new WaitForSeconds(waitTimeTurn);

			//Turn left
			while(!IsAlmostEqual(transform.forward, leftTurnBorder))
			{
				TurnLeftTowards(leftTurnBorder);
				yield return null;
			}

			yield return new WaitForSeconds(waitTimeTurn);
		}
		
	}

	IEnumerator WalkOnPath(int startWaypoint)
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

	public bool HasTurnBehavior()
	{
		return idleBehavior == IdleBehavior.Turn;
	}

	#endregion

	IEnumerator DetectedFrank()
	{
		currentBehavior = Behavior.DetectedFrank;

		//Say Frank, he was detected
		frank.OnDetect();

		//Turn towards Frank
		while (!LookingAt(frank.transform.position))
		{
			TurnTowardsSmooth(frank.transform.position, true);
			yield return null;
		}

		//Move towards Frank
		while (!IsTouching(frank.gameObject))
		{
			MoveTowards(frank.transform.position);
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);

		//Move towards last checkpoint
		agent.enabled = true;

		Transform lastCheckpoint = Checkpoint.lastCheckpoint.transform;
		if (!agent.SetDestination(lastCheckpoint.position))
		{
			Debug.LogError("No way to the last checkpoint was found!");
		}

		//wait for reaching the destination
		while(!IsAbove(lastCheckpoint.position, transform.position))
		{
			yield return null;
		}

		yield return new WaitForSeconds(1f);

		//Move back to normal position
		if (idleBehavior == IdleBehavior.WalkOnPath)
		{
			//Find nearest waypoint
			NavMeshPath path = new NavMeshPath();
			Vector3 nearestWaypoint = waypoints[0];
			int indexOfNearestWaypoint = 0;
			float shortestPathLength = Mathf.Infinity;
			for (int i = 0; i < waypoints.Length; i++)
			{
				agent.CalculatePath(waypoints[i], path);
				float pathLength = GetPathLength(path);

				if(pathLength < shortestPathLength)
				{
					shortestPathLength = pathLength;
					indexOfNearestWaypoint = i;
					nearestWaypoint = waypoints[i];
				}
			}

			//Go to nearest Waypoint
			if (!agent.SetDestination(nearestWaypoint))
			{
				Debug.LogError("No way to the start position was found!");
			}

			//wait for reaching the destination
			while (!IsAbove(nearestWaypoint, transform.position))
			{
				yield return null;
			}

			Idle(indexOfNearestWaypoint + 1);

		} else
		{
			if (!agent.SetDestination(startPosition))
			{
				Debug.LogError("No way to the start position was found!");
			}

			//wait for reaching the destination
			while (!IsAbove(startPosition, transform.position))
			{
				yield return null;
			}

			while(!IsAbove(startForwardDirection, transform.forward, 0.01f)){
				TurnTowardsSmooth(startForwardDirection);
				yield return null;
			}

			//Go back to Idle
			Idle(0);
		}

		agent.enabled = false;
	}

	#endregion

	#region Behavior transitions

	void OnDectectFrank()
	{
		//Check if frank is already detected (regardless of who detected him)
		if(!frank.IsDetected())
		{
			currentBehavior = Behavior.DetectedFrank;
			frank.OnDetect();
		}
	}

	#endregion

	private void OnDrawGizmos()
	{
		//Draw Path, if Idle Behavior = MoveOnPath
		if(idleBehavior == IdleBehavior.WalkOnPath)
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
