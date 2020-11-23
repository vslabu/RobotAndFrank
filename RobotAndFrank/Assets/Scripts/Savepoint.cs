using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Savepoint : MonoBehaviour
{
	public static Savepoint lastSavepoint;
	public bool isStart;

	private void Start()
	{
		if(isStart && lastSavepoint == null)
		{
			lastSavepoint = this;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			lastSavepoint = this;
		}
		
	}
}
