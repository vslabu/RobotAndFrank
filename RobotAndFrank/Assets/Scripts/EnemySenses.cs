using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemySenses : NPC_Controller
{
	protected Frank_Controller frank;

	[Header("FoV Variables")]
	public float viewRadius = 10f;
	[Range(0,360)]
	public float viewAngle;
	[SerializeField]
	protected LayerMask obstacleMask;


	protected void InitSenses()
    {
		InitNPCMovement();
		GameObject frankObject = GameObject.FindGameObjectWithTag("Frank");
		frank = frankObject.GetComponent<Frank_Controller>();
	}

	protected bool CanSeeFrank()
	{
		if(!CanSeePosition(frank.transform.position, viewRadius, obstacleMask)){
			//Frank is either too far away or hidden by an obstacle
			return false;
		}

		Vector3 directionToFrank = (frank.transform.position - transform.position).normalized;
		if(Vector3.Angle(transform.forward, directionToFrank) < viewAngle / 2)
		{
			//Frank is in the viewangle of the guard
			return true;
		}

		return false;
	}

	public Vector3 DirFromAngle (float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
