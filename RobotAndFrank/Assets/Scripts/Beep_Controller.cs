﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beep_Controller : MonoBehaviour
{

	bool buttonDown = false;
	float timer_down = 0;
	[SerializeField]
	readonly float shortPressTime = 0.45f; //Pressing the beep-button shorter than this is a short beep. Otherwise it's a long one

	// Update is called once per frame
	void Update()
    {
		if (Input.GetButtonDown("Beep"))
		{
			GameEvents.current.BeepStart(transform.position);
			buttonDown = true;
		} else if (Input.GetButtonUp("Beep" ))
		{
			buttonDown = false;
		}
    }

	private void FixedUpdate()
	{
		
		if(buttonDown == true)
		{	
			//Counting the time of the button press
			timer_down += Time.fixedDeltaTime;
		} else if(timer_down > 0)
		{
			if(timer_down >= shortPressTime)
			{
				GameEvents.current.LongBeep(transform.position);
				timer_down = 0f;
			} else if(timer_down < shortPressTime)
			{
				GameEvents.current.ShortBeep(transform.position);
				timer_down = 0f;
			}
		}
	}
}
