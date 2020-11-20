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

		Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
		Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

		//Draw Circle
		Handles.color = Color.white;
		Handles.DrawWireArc(fov.transform.position, Vector3.up, viewAngleA, fov.viewAngle, fov.viewRadius);

		//Draw Lines
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);
	}
}
