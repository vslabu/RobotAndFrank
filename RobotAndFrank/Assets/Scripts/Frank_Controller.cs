using System.Collections;
using UnityEngine;

public class Frank_Controller : NPC_Controller
{
	enum Behavior
	{
		Idle,
		Walk,
		Detected
	};

	Behavior currentBehavior = Behavior.Idle;

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

	#region Behaviors
	IEnumerator Idle()
	{
		currentBehavior = Behavior.Idle;

		//TODO: Implement idle behavior
		yield return null;
	}

	IEnumerator Walk(Vector3 destination)
	{
		currentBehavior = Behavior.Walk;

		//Turn towards Destination
		Vector3 offset = destination - transform.position;
		while (!LooksAt(destination))
		{
			TurnTowardsSmooth(offset);
			yield return null;
		}

		while(!IsAbove(destination, transform.position))
		{
			MoveTowards(destination);
			yield return null;
		}

		ChangeCurrentCoroutine(Idle());
	}

	IEnumerator Detected()
	{
		currentBehavior = Behavior.Detected;

		//TODO: implement behavior when detected
		yield return null;
	}

	#endregion

	#region Detecting external events
	void OnShortBeep(Vector3 beepPos)
	{
		//Check if Frank can hear the sound
		if (CanHearBeep(beepPos))
		{
			ChangeCurrentCoroutine(Idle());
		}
	}

	void OnLongBeep(Vector3 beepPos)
	{
		//Check if Frank can hear the sound
		if(CanHearBeep(beepPos))
		{
			ChangeCurrentCoroutine(Walk(beepPos));
		}
	}

	public void OnDetect()
	{
		ChangeCurrentCoroutine(Detected());
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//Stop walking towards the destination, when touching the player
		if(hit.gameObject.tag == "Player")
		{
			ChangeCurrentCoroutine(Idle());
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
