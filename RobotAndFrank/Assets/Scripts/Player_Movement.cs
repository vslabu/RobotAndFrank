using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
	CharacterController controller;
	Vector3 movement_direction;

	[SerializeField]
	float movementSpeed = 5f;
	[SerializeField]
	float turningTime = 1f;
	float turnSmoothVelocity;

	private void Start()
	{
		controller = GetComponentInParent<CharacterController>();
	}

	private void Update()
	{
		movement_direction.x = Input.GetAxis("Horizontal");
		movement_direction.z = Input.GetAxis("Vertical");
		if (movement_direction.magnitude >= 0.1f)
		{
			//Turn Player in the direction he's walking
			float targetAngle = Mathf.Atan2(movement_direction.x, movement_direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turningTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);

			//Move Player
			controller.SimpleMove(movement_direction * movementSpeed);
		}
		
	}
}
