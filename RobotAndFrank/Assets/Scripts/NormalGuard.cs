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

	#region TurnBehavior

	[Range(0,360)]
	public float turnAngle = 0; //Only used if idleBehavior = Turn
	bool turnright = true;
	[HideInInspector]
	public Vector3 leftTurnBorder, rightTurnBorder;

	#endregion

	#endregion

	// Start is called before the first frame update
	void Start()
    {
		InitSenses();

		//Init vectors used in turning behavior
		leftTurnBorder = transform.forward;
		rightTurnBorder = DirFromAngle(turnAngle, false);
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
				Turn();
				return; //TODO: Guard turning
			case IdleBehavior.MoveOnPath:
				return; //TODO: Guard moving on path
		}
	}

	void Turn()
	{
		if (turnright)
		{
			//Debug.Log("Rechts");
			TurnRightTowards(rightTurnBorder);
			if(IsAlmostEqual(transform.forward, rightTurnBorder))
			{
				//Reached right border
				turnright = false;
			}
		} else
		{
			//Debug.Log("Links");
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
