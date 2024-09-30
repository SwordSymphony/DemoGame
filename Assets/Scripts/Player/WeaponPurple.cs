using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPurple : MonoBehaviour
{
	public static AudioManager audioManager;
	public Rigidbody2D rb;
	public Animator animator;
	public GameObject player;
	LayerMask enemyLayerMask;
	public int projectileDamage;
	public float aoeSize;
	public float impactSize;
	public int fearDuration;

	public bool isFlipped;

	public float detectRadius;
	public float speed;
	public int force;
	bool targetFound;
	Collider2D enemy;

	Vector2 initialVector;
	
	ContactFilter2D contactFilter;
	// public LayerMask PlayerLayer;

	bool triggered;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("DarkCast");
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>(); 
		enemyLayerMask = LayerMask.GetMask("PurpleEnemyLayer");

		aoeSize = player.GetComponent<Player>().darkAoeSize;
		impactSize = player.GetComponent<Player>().darkImpactSize;
		projectileDamage = player.GetComponent<Player>().damageDark;
		detectRadius = 15.0f;
		speed = 30.0f;

		fearDuration = 4;
		force = 5;
		Destroy(gameObject, 2.5f);
		initialVector = rb.velocity;
	}

	void FixedUpdate()
	{
		if (enemy == null)
		{
			FindTarget();
		}

		if (enemy != null && !triggered)
		{
			transform.position = Vector2.MoveTowards(transform.position, enemy.transform.position, speed * Time.deltaTime);
			Vector3 direction = enemy.transform.position - transform.position;
			transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
		}

		if (!triggered && rb.velocity == Vector2.zero && enemy == null)
		{
			// Destroy(gameObject);
			rb.velocity = initialVector;
		}
	}

	void FindTarget()
	{
		enemy = Physics2D.OverlapCircle(transform.position, detectRadius, enemyLayerMask);
		if (enemy is not null)
		{
			rb.velocity = Vector3.zero;
		}
	}

	void OnTriggerEnter2D()
	{
		if (!triggered)
		{
			List <Collider2D> results = new List<Collider2D>();
			int num = Physics2D.OverlapCircle(transform.position, aoeSize, contactFilter, results);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					switch (results[i].gameObject.tag)
					{
						case "BlueEnemy":
							results[i].gameObject.GetComponent<EnemyBlue>().Fear(fearDuration);
							results[i].gameObject.GetComponent<EnemyBlue>().Knockback(force, transform.position);
							if (results[i].gameObject.GetComponent<EnemyBlue>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyBlue>().TakeDamage(projectileDamage);
							}
							break;
						case "GreenEnemy":
							results[i].gameObject.GetComponent<EnemyGreen>().Fear(fearDuration);
							results[i].gameObject.GetComponent<EnemyGreen>().Knockback(force, transform.position);
							if (results[i].gameObject.GetComponent<EnemyGreen>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyGreen>().TakeDamage(projectileDamage);
							}
							break;
						case "PurpleEnemy":
							results[i].gameObject.GetComponent<EnemyPurple>().TakeDamage(projectileDamage);
							results[i].gameObject.GetComponent<EnemyPurple>().Fear(fearDuration);
							results[i].gameObject.GetComponent<EnemyPurple>().Knockback(force, transform.position);
							break;
						case "RedEnemy":
							results[i].gameObject.GetComponent<EnemyRed>().Fear(fearDuration);
							results[i].gameObject.GetComponent<EnemyRed>().Knockback(force, transform.position);
							if (results[i].gameObject.GetComponent<EnemyRed>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyRed>().TakeDamage(projectileDamage);
							}
							break;
						case "YellowEnemy":
							results[i].gameObject.GetComponent<EnemyYellow>().Fear(fearDuration);
							results[i].gameObject.GetComponent<EnemyYellow>().Knockback(force, transform.position);
							if (results[i].gameObject.GetComponent<EnemyYellow>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
							}
							break;
					}
				}
			}
			if (isFlipped)
			{
				GetComponent<SpriteRenderer>().flipY = false;
			}
			triggered = true;
			animator.SetBool("impact", true);
			transform.rotation = Quaternion.identity;
			rb.velocity = Vector3.zero;
			transform.localScale = new Vector3(impactSize, impactSize, 0);
		}
	}

	public void SelfDestruct()
	{
		Destroy(gameObject);
	}
}

