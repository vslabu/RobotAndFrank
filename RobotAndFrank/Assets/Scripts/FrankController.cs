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

	[Header("Hearing Variables")]
	public float hearingDistance = 20f;
	public LayerMask hearingMask;


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
		//Check if Frank reached the destination
		float distanceToCurrentDestination = Distance(currentDestination, transform.position);
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

	void OnShortBeep(Vector3 beepPos)
	{
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

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Stop walking towards the destination, when touching the player
		if(hit.gameObject.tag == "Player")
		{
			currentBehavior = Behavior.Stand;
		}
	}

	private bool CanHearBeep(Vector3 beepPos)
	{
		return CanSeePosition(beepPos, hearingDistance, hearingMask);
	}
}
