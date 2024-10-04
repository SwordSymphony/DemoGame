using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public GameObject player;
	Vector2 mousePos;
	public Camera cam;
	float moveSpeed;
	float moveSpeedBack;

	float moveSpeedDash;

	void Start()
	{
		moveSpeed = 30.0f;
		moveSpeedBack = 50.0f;
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -25);
	}

	void Update()
	{
		mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
	}

	void FixedUpdate()
	{

		float distancePlayerToMouse = Vector3.Distance(player.transform.position, mousePos);

		Vector3 directionToMouse = new Vector3(mousePos.x - player.transform.position.x, mousePos.y - player.transform.position.y, -25); // from player to mouse direction
		directionToMouse.Normalize();

		if (distancePlayerToMouse > 28)
		{
			Vector3 direction = new Vector3(player.transform.position.x + 12 * directionToMouse.x, player.transform.position.y + 12 * directionToMouse.y, -25);
			transform.position = Vector3.MoveTowards(transform.position, direction, moveSpeed * Time.deltaTime);
		}
		else if (distancePlayerToMouse < 28)
		{
			// move camera to player
			Vector3 direction = new Vector3(player.transform.position.x, player.transform.position.y, -25);
			transform.position = Vector3.MoveTowards(transform.position, direction, moveSpeedBack * Time.deltaTime);
		}
	}

	public void CameraSpeed(float speed)
	{
		moveSpeed = speed;
	}
}
