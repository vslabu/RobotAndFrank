using UnityEngine;

public class Frank_Controller : NPC_Controller
{
	enum Behavior
	{
		Stand,
		Walk,
		Detected
	};

	Behavior currentBehavior = Behavior.Stand;
	Vector3 currentDestination;

	[Header("Hearing Variables")]
	[SerializeField]
	float hearingRadius = 20f;
	[SerializeField]
	public LayerMask obstacleMask;


	// Start is called before the first frame update
	void Start()
    {
		InitNPCMovement();
		GameEvents.current.OnShortBeep += OnShortBeep;
		GameEvents.current.OnLongBeep += OnLongBeep;
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

	#region Behaviors

	void WalkBehavior()
	{
		//Check if Frank reached the destination
		float distanceToCurrentDestination = Vector3.Distance(currentDestination, transform.position);
		if (distanceToCurrentDestination > epsilon)
		{
			//Did not yet reach it
			MoveTowards(currentDestination);
		} else
		{
			//Reached the destination and changes to mode Stand
			currentBehavior = Behavior.Stand;
		}
	}

	#endregion

	#region Detecting external events
	void OnShortBeep(Vector3 beepPos)
	{
		//Check if Frank can hear the sound
		if (CanHearBeep(beepPos))
		{
			currentBehavior = Behavior.Stand;
		}
	}

	void OnLongBeep(Vector3 beepPos)
	{
		//Check if Frank can hear the sound
		if(CanHearBeep(beepPos))
		{
			currentBehavior = Behavior.Walk;
			currentDestination = beepPos;
		}
	}

	public void OnDetect()
	{
		currentBehavior = Behavior.Detected;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Stop walking towards the destination, when touching the player
		if(hit.gameObject.tag == "Player")
		{
			currentBehavior = Behavior.Stand;
		}
	}

	#endregion

	private bool CanHearBeep(Vector3 beepPos)
	{
		return CanSeePosition(beepPos, hearingRadius, obstacleMask);
	}

	public bool IsDetected()
	{
		return currentBehavior == Behavior.Detected;
	}
}
