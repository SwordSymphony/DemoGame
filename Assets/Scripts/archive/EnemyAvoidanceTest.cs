using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAvoidanceTest : MonoBehaviour
{
	public Rigidbody2D rb;
	public float moveSpeed = 1f;

	// Pathfinder
	public GameObject player;

	// Health
	public int maxHealth = 100;
	public int currentHealth;
	public HealthBar healthbar;

	public bool hasLineOfSight = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		player = GameObject.FindGameObjectWithTag("Player");

		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);
	}

	// Update is called once per frame
	void Update()
	{
		// RaycastHit2D Raycast(rb.position, player, 10000.0f, PlayerLayer, 0.0f, 0.0f)
	}

	void FixedUpdate()
	{
		// moveToPlayer();
		pathFind();
	}

	private void pathFind()
	{
		Vector3 vectorTopRight = (transform.right*4 + new Vector3(0, 6, 0)).normalized * 6;
		Vector3 vectorTopLeft = (-transform.right*4 + new Vector3(0, 6, 0)).normalized * 6;
		Vector3 vectorDownRight = (transform.right*4 + new Vector3(0, -6, 0)).normalized * 6;
		Vector3 vectorDownLeft = (-transform.right*4 + new Vector3(0, -6, 0)).normalized * 6;
		Vector3 vectorRight = (transform.right*4).normalized * 6;
		Vector3 vectorLeft = (-transform.right*4).normalized * 6;

		Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized*6, Color.red); // line to player
	
		Debug.DrawRay(transform.position, vectorTopRight, Color.blue);
		Debug.DrawRay(transform.position, vectorTopLeft, Color.blue);
		Debug.DrawRay(transform.position, vectorDownLeft, Color.blue);
		Debug.DrawRay(transform.position, vectorDownRight, Color.blue);
		Debug.DrawRay(transform.position, vectorLeft, Color.blue);
		Debug.DrawRay(transform.position, vectorRight, Color.blue);

		LayerMask mask = LayerMask.GetMask("Walls");

		RaycastHit2D rayToPlayer = Physics2D.Raycast(transform.position, player.transform.position - transform.position, 6.0f, mask); // Raycast to player but fixed distance.

		RaycastHit2D rayUpRight = Physics2D.Raycast(transform.position, transform.position + vectorTopRight, 6.0f, mask); // Raycast to up right from player
		RaycastHit2D rayUpLeft = Physics2D.Raycast(transform.position, transform.position + vectorTopLeft, 6.0f, mask); // Raycast to up left from player
		RaycastHit2D rayDownRight = Physics2D.Raycast(transform.position, transform.position + vectorDownRight, 6.0f, mask); // Raycast to down right from player
		RaycastHit2D rayDownLeft = Physics2D.Raycast(transform.position, transform.position + vectorDownLeft, 6.0f, mask); // Raycast to down left from player
		RaycastHit2D rayRight = Physics2D.Raycast(transform.position, transform.position + vectorRight, 6.0f, mask); // Raycast to right from player
		RaycastHit2D rayLeft = Physics2D.Raycast(transform.position, transform.position + vectorLeft, 6.0f, mask); // Raycast to left from player

		float distance1 = Vector3.Distance(player.transform.position, transform.position + vectorTopRight);
		float distance2 = Vector3.Distance(player.transform.position, transform.position + vectorTopLeft);
		float distance3 = Vector3.Distance(player.transform.position, transform.position + vectorDownRight);
		float distance4 = Vector3.Distance(player.transform.position, transform.position + vectorDownLeft);
		float distance5 = Vector3.Distance(player.transform.position, transform.position + vectorLeft);
		float distance6 = Vector3.Distance(player.transform.position, transform.position + vectorRight);

		float minimumDistance = Mathf.Min(distance1, distance2, distance3, distance4, distance5, distance6);

		Vector3 movementDirection;// = player.transform.position;

		if (rayToPlayer.collider is null) // if no collider to player
		{
			// Move towards the player
			movementDirection = player.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, movementDirection, moveSpeed * Time.deltaTime);
		}
		else // get closest vector
		{
			SortedSet<float> distances = new SortedSet<float>();
			distances.Add(distance1);
			distances.Add(distance2);
			distances.Add(distance3);
			distances.Add(distance4);
			distances.Add(distance5);
			distances.Add(distance6);

			GetVector:
			switch (minimumDistance)
			{
				case var value when value == distance1:
					if (rayUpRight.collider is null)
					{
						movementDirection = vectorTopRight;
						transform.position = Vector3.MoveTowards(transform.position, transform.position + movementDirection, moveSpeed * Time.deltaTime);

						Debug.Log(movementDirection+ "moving to distance 1");
						Debug.DrawRay(transform.position, movementDirection, Color.green);
						break;
					}
					distances.Remove(distance1);
					minimumDistance = distances.Min;
					goto GetVector;

				case var value when value == distance2:
					if (rayUpLeft.collider is null)
					{
						movementDirection = vectorTopLeft;
						transform.position = Vector3.MoveTowards(transform.position, transform.position + movementDirection, moveSpeed * Time.deltaTime);

						Debug.Log(movementDirection+"moving to distance 2");
						Debug.DrawRay(transform.position, movementDirection, Color.green);
						break;
					}
					distances.Remove(distance2);
					minimumDistance = distances.Min;
					goto GetVector;

				case var value when value == distance3:
					if (rayDownRight.collider is null)
					{
						movementDirection = vectorDownRight;
						transform.position = Vector3.MoveTowards(transform.position, transform.position + movementDirection, moveSpeed * Time.deltaTime);

						Debug.Log(movementDirection+"moving to distance 3");
						Debug.DrawRay(transform.position, movementDirection, Color.green);
						break;
					}
					distances.Remove(distance3);
					minimumDistance = distances.Min;
					goto GetVector;

				case var value when value == distance4:
					if (rayDownLeft.collider is null)
					{
						movementDirection = vectorDownLeft;
						transform.position = Vector3.MoveTowards(transform.position, transform.position + movementDirection, moveSpeed * Time.deltaTime);

						Debug.Log(movementDirection+"moving to distance 4");
						Debug.DrawRay(transform.position, movementDirection, Color.green);
						break;
					}
					distances.Remove(distance4);
					minimumDistance = distances.Min;
					goto GetVector;

				case var value when value == distance5:
					if (rayLeft.collider is null)
					{
						movementDirection = vectorLeft;
						transform.position = Vector3.MoveTowards(transform.position, transform.position + movementDirection, moveSpeed * Time.deltaTime);

						Debug.Log(movementDirection+"moving to distance 5");
						Debug.DrawRay(transform.position, movementDirection, Color.green);
						break;
					}
					distances.Remove(distance5);
					minimumDistance = distances.Min;
					goto GetVector;

				case var value when value == distance6:
					if (rayRight.collider is null)
					{
						movementDirection = vectorRight;
						transform.position = Vector3.MoveTowards(transform.position, transform.position + movementDirection, moveSpeed * Time.deltaTime);

						Debug.Log(movementDirection+"moving to distance 6");
						Debug.DrawRay(transform.position, movementDirection, Color.green);
						break;
					}
					distances.Remove(distance6);
					minimumDistance = distances.Min;
					goto GetVector;
			}
		}
	}

	// private void AttackPlayer()
	// {
	// }

	public void TakeDamage(int damage)
	{
		currentHealth -= damage;
		healthbar.SetHealth(currentHealth);

		Debug.Log("Took"+ damage+"damage");

		if(currentHealth <= 0)
		{
			Die();
		}
	}

	public void Die()
	{
		Debug.Log("Enemy died");

		Destroy(gameObject);
	}

}
