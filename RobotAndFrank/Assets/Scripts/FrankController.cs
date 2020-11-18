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
		InitSuper();
		GameEvents.current.onShortBeep += OnShortBeep;
		GameEvents.current.onLongBeep += OnLongBeep;
	}


	void Update()
	{
		switch (currentBehavior)
		{
			case Behavior.Walk:
				WalkBehavior();
				return;
		}	
	}

	void WalkBehavior()
	{
		Vector3 offset = currentDestination - transform.position;
		if (offset.magnitude > epsilon)
		{
			MoveTowards(currentDestination);
		} else
		{
			currentBehavior = Behavior.Stand;
		}
	}

	void OnShortBeep(Vector3 beepPos)
	{
		currentBehavior = Behavior.Stand;
	}

	void OnLongBeep(Vector3 beepPos)
	{
		currentBehavior = Behavior.Walk;
		currentDestination = beepPos;
	}
}
