using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(EnemySenses), true)]
public class NewBehaviourScript : Editor
{
	private void OnSceneGUI()
	{
		EnemySenses fov = (EnemySenses)target;

		#region Draw field of view
		Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
		Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

		//Draw Circle
		Handles.color = Color.white;
		Handles.DrawWireArc(fov.transform.position, Vector3.up, viewAngleA, fov.viewAngle, fov.viewRadius);

		//Draw Lines
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);
		#endregion

		#region Draw turn angle if needed
		if (fov is NormalGuard guard && guard.HasTurnBehavior())
		{
			Handles.color = Color.green;

			//Draw borders of turning
			Vector3 turnAngleA, turnAngleB;
			if (!Application.isPlaying)
			{
				//Used in Editor
				turnAngleA = guard.transform.forward;
				turnAngleB = guard.DirFromAngle(guard.turnAngle, false);
			}
			else
			{
				turnAngleA = guard.leftTurnBorder;
				turnAngleB = guard.rightTurnBorder;
			}

			Handles.DrawLine(guard.transform.position, guard.transform.position + turnAngleA * guard.viewRadius / 2);
			Handles.DrawLine(guard.transform.position, guard.transform.position + turnAngleB * guard.viewRadius / 2);

			//Draw circle
			Handles.DrawWireArc(guard.transform.position, Vector3.up, turnAngleA, guard.turnAngle, guard.viewRadius / 2);

			//Draw Guards forward direction
			Handles.color = Color.red;
			Handles.DrawLine(guard.transform.position, guard.transform.position + guard.transform.forward * guard.viewRadius / 2);
		}
		#endregion
	}
}
