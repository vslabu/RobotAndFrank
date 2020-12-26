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

	[Header("Behavior")]
	Behavior currentBehavior = Behavior.Idle;

	#endregion

	// Start is called before the first frame update
	void Start()
    {
		InitSenses();

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

	IEnumerator DetectedFrank()
	{
		currentBehavior = Behavior.DetectedFrank;

		//Say Frank, he was detected
		frank.OnDetect();

		//Turn towards Frank
		yield return SubChangeCurrentCoroutine(TurnTowardsCoroutine(frank.transform.position));

		//Move towards Frank
		yield return SubChangeCurrentCoroutine(MoveToObjectCoroutine(frank.gameObject));

		yield return new WaitForSeconds(0.3f);

		//Tell Frank he has to follow
		frank.OnStartEscorting(this);

		//Move towards last checkpoint
		Vector3 lastCheckpoint = Checkpoint.lastCheckpoint.transform.position;
		yield return SubChangeCurrentCoroutine(PathfindTowards(lastCheckpoint));

		//Tell Frank he can stop following
		frank.OnFinishedEscorting();
		yield return new WaitForSeconds(1f);

		//Move back to normal position
		if (idleBehavior == IdleBehavior.WalkOnPath)
		{
			//Go to nearest Waypoint
			int nearestWaypoint = FindNearestWaypoint();

			yield return SubChangeCurrentCoroutine(PathfindTowards(waypoints[nearestWaypoint]));

			Idle(nearestWaypoint + 1);

		} else
		{
			//Move towards Start Position
			yield return SubChangeCurrentCoroutine(PathfindTowards(startPosition));

			//Turn towards Start Position
			yield return SubChangeCurrentCoroutine(TurnTowardsCoroutine(transform.position + startForwardDirection));

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
		}
	}

	#endregion
}
