using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public static Checkpoint lastCheckpoint;
	public bool isStart;

	private void Start()
	{
		if(isStart && lastCheckpoint == null)
		{
			lastCheckpoint = this;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			lastCheckpoint = this;
		}
		
	}
}
