using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBase : MonoBehaviour
{
	public Rigidbody2D rb;

	// public float attackRange;
	// public bool playerInAttackRange;
	public float baseMoveSpeed;
	public float currentMoveSpeed;

	public GameObject player;

	public bool isSlowed;
	public bool isFrozen;
	public bool isFeared;
	public bool isPushed;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		player = GameObject.FindWithTag("Player");

		baseMoveSpeed = 3.0f;
		currentMoveSpeed = baseMoveSpeed;
	}

	// Update is called once per frame
	void Update()
	{
		// while (playerInAttackRange) 
		// {
		// 	AttackPlayer();
		// 	//invoke every sec?
		// }
	}

	void FixedUpdate()
	{
		if (!isFrozen)
		{
			if (isFeared)
			{
				Vector2 movementDirection = GetOppositeVector();
				transform.position = Vector2.MoveTowards(transform.position, movementDirection, currentMoveSpeed * Time.deltaTime);
			}
			else
			{
				transform.position = Vector2.MoveTowards(transform.position, player.transform.position, currentMoveSpeed * Time.deltaTime);
			}
			
		}
	}

	public IEnumerator FreezeMovement(int seconds)
	{
		isFrozen = true;
		//Freeze all positions
		rb.constraints = RigidbodyConstraints2D.FreezePosition;

		yield return new WaitForSeconds(seconds);
		rb.constraints = RigidbodyConstraints2D.None; // remove freeze
		rb.constraints = RigidbodyConstraints2D.FreezeRotation; // get freeze rotation back
	}

	public IEnumerator SlowMovement(int seconds)
	{
		isSlowed = true;
		currentMoveSpeed = baseMoveSpeed / 2;
		yield return new WaitForSeconds(seconds);
		isSlowed = false;
		currentMoveSpeed = baseMoveSpeed;
	}

	public IEnumerator SetFear(int seconds)
	{
		isFeared = true;
		currentMoveSpeed = baseMoveSpeed * 2;
		yield return new WaitForSeconds(seconds);
		isFeared = false;
		currentMoveSpeed = baseMoveSpeed;
	}

	public IEnumerator Push(int force)
	{
		isPushed = true;
		Vector2 direction = GetOppositeVector();
		rb.AddForce(direction * force, ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.1f);
		isPushed = false;
		rb.velocity = Vector3.zero;
	}

	public Vector2 GetOppositeVector()
	{
		float posX = transform.position.x;
		float posY = transform.position.y;

		if (player.transform.position.x >= transform.position.x)
		{
			posX = player.transform.position.x * - 1;
		}
		else if (player.transform.position.x <= transform.position.x)
		{
			posX = player.transform.position.x * 1;
		}

		if (player.transform.position.y >= transform.position.y)
		{
			posY = player.transform.position.y * - 1;
		}
		else if (player.transform.position.y <= transform.position.y)
		{
			posY = player.transform.position.y * 1;
		}
		Vector2 movementDirection = new Vector2(posX, posY);
		return movementDirection;
	}

	private void AttackPlayer()
	{
		// Debug.Log("attack");
	}
}
