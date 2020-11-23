using UnityEngine;
using UnityEngine.AI;

public class NormalGuard : EnemySenses
{
	#region Behavior

	enum Behavior
	{
		Idle, //Normal behavior, see enum IdleBehavior
		Suspicious, //Hears something, checks out the source
		DetectedFrank, //Seeing Frank and going to him
		BringingFrankToCheckpoint, //Brings Frank back to the last Checkpoint
		BackToBuisness //Guard goes Back to his normal buisness
	}

	enum IdleBehavior
	{
		Stand,
		Turn,
		MoveOnPath
	}

	[Header("Behavior")]
	[SerializeField]
	IdleBehavior idleBehavior = IdleBehavior.Stand;
	Behavior currentBehavior = Behavior.Idle;

	#region Turn Behavior

	[Range(0,360)]
	public float turnAngle = 0; //Only used if idleBehavior = Turn
	bool turnright = true;
	[HideInInspector]
	public Vector3 leftTurnBorder, rightTurnBorder;
	Vector3 startPosition;

	#endregion

	#region WalkOnPath Behavior

	public Transform pathHolder;
	Vector3[] waypoints;
	int targetWaypoint = 0;

	#endregion

	#endregion


	NavMeshAgent agent;

	// Start is called before the first frame update
	void Start()
    {
		InitSenses();

		//Init vectors used in turning behavior
		leftTurnBorder = transform.forward;
		rightTurnBorder = DirFromAngle(turnAngle, false);

		//Init path
		if(pathHolder != null)
		{
			waypoints = new Vector3[pathHolder.childCount];
			for (int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i] = pathHolder.GetChild(i).position;
			}
		}

		//Init startPosition
		startPosition = transform.position;

		//Init NavMeshAgent
		agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
		//Check if guard can see Frank
		if (CanSeeFrank())
		{
			OnDectectFrank();
		}

		switch (currentBehavior)
		{
			case Behavior.Idle:
				Idle();
				return;
			case Behavior.DetectedFrank:
				DetectedFrank();
				return;
			case Behavior.BringingFrankToCheckpoint:
				BringFrankToCheckpoint();
				return;
			case Behavior.BackToBuisness:
				BackToBuisness();
				return;
		}
	}

	#region Behaviors

	#region Idle Behaviors

	private void Idle()
	{
		switch (idleBehavior)
		{
			case IdleBehavior.Stand:
				return;
			case IdleBehavior.Turn:
				Turn();
				return;
			case IdleBehavior.MoveOnPath:
				MoveOnPath();
				return;
		}
	}

	void Turn()
	{
		if (turnright)
		{
			TurnRightTowards(rightTurnBorder);
			if(IsAlmostEqual(transform.forward, rightTurnBorder))
			{
				//Reached right border
				turnright = false;
			}
		} else
		{
			TurnLeftTowards(leftTurnBorder);
			if (IsAlmostEqual(transform.forward, leftTurnBorder))
			{
				//reached left border
				turnright = true;
			}
		}
	}

	public bool HasTurnBehavior()
	{
		return idleBehavior == IdleBehavior.Turn;
	}


	void MoveOnPath()
	{
		MoveTowards(waypoints[targetWaypoint]);
		if(IsAbove( transform.position, waypoints[targetWaypoint]))
		{
			targetWaypoint = (targetWaypoint + 1) % waypoints.Length;
		}
	}

	#endregion

	void DetectedFrank()
	{
		MoveTowards(frank.transform.position);
	}

	void BringFrankToCheckpoint()
	{
		//check if guard does already have a destination
		if(!agent.hasPath)
		{
			//Set destination
			Transform lastCheckpoint = Savepoint.lastCheckpoint.transform;
			if(lastCheckpoint != null)
			{
				agent.SetDestination(lastCheckpoint.position);
				Debug.Log("Start");
			}
		}

		//check if guard reached the checkpoint
		float dist = agent.remainingDistance;
		if (agent.hasPath && dist == 0)
		{
			//currentBehavior = Behavior.BackToBuisness;
			Debug.Log("Ziel");
		}
	}

	void BackToBuisness()
	{
		//check if guard reached his start position
		if (agent.pathStatus == NavMeshPathStatus.PathComplete)
		{
			currentBehavior = Behavior.Idle;
		}

		//check if guard does already have a destination
		if (agent.pathStatus != NavMeshPathStatus.PathPartial)
		{
			//Set destination
			agent.SetDestination(startPosition);
			Debug.Log("Test");
		}
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

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Stop walking towards the Frank, when touching the Frank
		if (currentBehavior == Behavior.DetectedFrank && hit.gameObject.tag == "Frank")
		{
			currentBehavior = Behavior.BringingFrankToCheckpoint;
		}
	}
	#endregion

	private void OnDrawGizmos()
	{
		//Draw Path, if Idle Behavior = MoveOnPath
		if(idleBehavior == IdleBehavior.MoveOnPath)
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
