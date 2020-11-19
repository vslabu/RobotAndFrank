using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{

	public static GameEvents current;

	private void Awake()
	{
		current = this;
	}

	public event Action<Vector3> OnBeepStart;
	public void BeepStart(Vector3 beepPos)
	{
		if(OnBeepStart != null)
		{
			OnBeepStart(beepPos);
		}
	}

	public event Action<Vector3> OnShortBeep;
	public void ShortBeep(Vector3 beepPos)
	{
		if (OnShortBeep != null)
		{
			OnShortBeep(beepPos);
		}
	}

	public event Action<Vector3> OnLongBeep;
	public void LongBeep(Vector3 beepPos)
	{
		if (OnLongBeep != null)
		{
			OnLongBeep(beepPos);
		}
	}

	public event Action<Vector3> OnFrankCaught;
	public void FrankCaught(Vector3 beepPos)
	{
		if (OnFrankCaught != null)
		{
			OnFrankCaught(beepPos);
		}
	}
}
