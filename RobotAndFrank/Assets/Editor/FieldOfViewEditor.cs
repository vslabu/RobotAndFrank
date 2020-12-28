using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(EnemySenses), true)]
public class FieldOfViewEditor : Editor
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
		if (fov.HasTurnBehavior())
		{
			Handles.color = Color.green;

			//Draw borders of turning
			Vector3 turnAngleA, turnAngleB;
			if (!Application.isPlaying)
			{
				//Used in Editor
				turnAngleA = fov.transform.forward;
				turnAngleB = fov.DirFromAngle(fov.turnAngle, false);
			}
			else
			{
				turnAngleA = fov.leftTurnBorder;
				turnAngleB = fov.rightTurnBorder;
			}

			Handles.DrawLine(fov.transform.position, fov.transform.position + turnAngleA * fov.viewRadius / 2);
			Handles.DrawLine(fov.transform.position, fov.transform.position + turnAngleB * fov.viewRadius / 2);

			//Draw circle
			Handles.DrawWireArc(fov.transform.position, Vector3.up, turnAngleA, fov.turnAngle, fov.viewRadius / 2);

			//Draw Guards forward direction
			Handles.color = Color.red;
			Handles.DrawLine(fov.transform.position, fov.transform.position + fov.transform.forward * fov.viewRadius / 2);
		}
		#endregion
	}
}
