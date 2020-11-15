using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrankController : NPC_Controller
{
	enum Behavior
	{
		Stand,
		Walk
	};

	Behavior currentBehavior = Behavior.Stand;
	Vector3 currentDestination;


	// Start is called before the first frame update
	void Start()
    {
		GameEvents.current.onShortBeep += OnShortStart;
		GameEvents.current.onLongBeep += OnLongStart;
	}


	void Update()
	{
		switch (currentBehavior)
		{
			case Behavior.Walk:
				walkBehavior();
				return;
		}	
	}

	void walkBehavior()
	{
		if(transform.position != currentDestination)
		{
			moveTowards(currentDestination);
		}
	}

	void OnShortStart(Vector3 beepPos)
	{
		currentBehavior = Behavior.Stand;
	}

	void OnLongStart(Vector3 beepPos)
	{
		currentBehavior = Behavior.Walk;
		currentDestination = vectorToPlaneVector(beepPos);
	}
}
