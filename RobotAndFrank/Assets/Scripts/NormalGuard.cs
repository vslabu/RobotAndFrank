using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGuard : EnemySenses
{
	#region Behavior Enums

	enum Behavior
	{
		Idle, //Normal behavior, see enum IdleBehavior
		Suspicios, //Hears something, checks out the source
		DetectedFrank, //Seeing Frank and going to him
		BringingFrankToCheckpoint, //Brings Frank back to the last Checkpoint
		BackToBuisness //Guard goes Back to his normal buisness
	}

	public enum IdleBehavior
	{
		Stand,
		Turn,
		MoveOnPath
	}

	Behavior currentBehavior = Behavior.Idle;
	public IdleBehavior idleBehavior = IdleBehavior.Stand;

	#endregion

	// Start is called before the first frame update
	void Start()
    {
		InitSenses();
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
		}
	}

	#region Behaviors

	private void Idle()
	{
		switch (idleBehavior)
		{
			case IdleBehavior.Stand:
				return;
			case IdleBehavior.Turn:
				return; //TODO: Guard turning
			case IdleBehavior.MoveOnPath:
				return; //TODO: Guard moving on path
		}
	}

	void DetectedFrank()
	{
		MoveTowards(frank.transform.position);
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

	#region Detect external Events

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Stop walking towards the Frank, when touching the Frank
		if (currentBehavior == Behavior.DetectedFrank && hit.gameObject.tag == "Frank")
		{
			currentBehavior = Behavior.BringingFrankToCheckpoint;
		}
	}

	#endregion
}
