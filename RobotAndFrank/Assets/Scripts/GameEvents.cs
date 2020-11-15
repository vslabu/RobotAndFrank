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

	public event Action<Vector3> onBeepStart;
	public void BeepStart(Vector3 beepPos)
	{
		if(onBeepStart != null)
		{
			onBeepStart(beepPos);
		}
	}

	public event Action<Vector3> onShortBeep;
	public void ShortBeep(Vector3 beepPos)
	{
		if (onShortBeep != null)
		{
			onShortBeep(beepPos);
		}
	}

	public event Action<Vector3> onLongBeep;
	public void LongBeep(Vector3 beepPos)
	{
		if (onLongBeep != null)
		{
			onLongBeep(beepPos);
		}
	}
}
