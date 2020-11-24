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

	// Start is called before the first frame update
	void Start()
    {
		InitNPCMovement();
		GameEvents.current.OnShortBeep += OnShortBeep;
		GameEvents.current.OnLongBeep += OnLongBeep;

		ChangeCurrentCoroutine(Idle());
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
		while (!LookingAt(destination))
		{
			TurnTowardsSmooth(destination, true);
			yield return null;
		}

		while(!IsAbove(destination, transform.position) && !IsTouching(player))
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
		if (CanHearBeep(beepPos) && currentBehavior != Behavior.Detected)
		{
			ChangeCurrentCoroutine(Idle());
		}
	}

	void OnLongBeep(Vector3 beepPos)
	{
		//Check if Frank can hear the sound
		if(CanHearBeep(beepPos) && currentBehavior != Behavior.Detected)
		{
			ChangeCurrentCoroutine(Walk(beepPos));
		}
	}

	public void OnDetect()
	{
		ChangeCurrentCoroutine(Detected());
	}

	#endregion

	private bool CanHearBeep(Vector3 beepPos)
	{
		return CanSeePosition(beepPos, hearingRadius, hearingObstacleMask);
	}

	public bool IsDetected()
	{
		return currentBehavior == Behavior.Detected;
	}
}
