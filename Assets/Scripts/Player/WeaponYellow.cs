using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponYellow : MonoBehaviour
{
	public static AudioManager audioManager;
	public Rigidbody2D rb;
	public Animator animator;
	public GameObject player;
	public int projectileDamage;
	public float aoeSize;
	public float impactSize;
	public int force;
	public int impactForce;
	
	ContactFilter2D contactFilter;
	// public LayerMask PlayerLayer;

	bool exploded;
	float penetrationCount;
	float penetrationCountMax;
	float overloadDuration;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("LightningCast");
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>(); 

		aoeSize = player.GetComponent<Player>().toxicAoeSize;
		impactSize = player.GetComponent<Player>().toxicImpactSize;
		projectileDamage = player.GetComponent<Player>().damageToxic;

		force = 5;
		impactForce = 15;
		overloadDuration = 10.0f;
		// contactFilter = ~PlayerLayer;
		penetrationCountMax = 3;

		Destroy(gameObject, 1);
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (penetrationCount < penetrationCountMax) // penetrate
		{
			float random = Random.Range(1, 100);
			switch (target.gameObject.tag)
				{
					case "BlueEnemy":
						target.gameObject.GetComponent<EnemyBlue>().Knockback(force, transform.position);
						if (random <= 10)
						{
							target.gameObject.GetComponent<EnemyBlue>().StartCoroutine("OverloadCoroutine", overloadDuration);
						}
						break;
					case "GreenEnemy":
						target.gameObject.GetComponent<EnemyGreen>().Knockback(force, transform.position);
						if (random <= 10)
						{
							target.gameObject.GetComponent<EnemyGreen>().StartCoroutine("OverloadCoroutine", overloadDuration);
						}
						break;
					case "PurpleEnemy":
						target.gameObject.GetComponent<EnemyPurple>().Knockback(force, transform.position);
						if (random <= 10)
						{
							target.gameObject.GetComponent<EnemyPurple>().StartCoroutine("OverloadCoroutine", overloadDuration);
						}
						break;
					case "RedEnemy":
						target.gameObject.GetComponent<EnemyRed>().Knockback(force, transform.position);
						if (random <= 10)
						{
							target.gameObject.GetComponent<EnemyRed>().StartCoroutine("OverloadCoroutine", overloadDuration);
						}
						break;
					case "YellowEnemy":
						target.gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
						target.gameObject.GetComponent<EnemyYellow>().Knockback(force, transform.position);
						if (random <= 10)
						{
							target.gameObject.GetComponent<EnemyYellow>().StartCoroutine("OverloadCoroutine", overloadDuration);
						}
						break;
					case "Obstacle":
						penetrationCount = penetrationCountMax;
						break;
				}
			penetrationCount++;
		}
		if (penetrationCount >= penetrationCountMax && !exploded)//(!triggered)
		{
			List <Collider2D> results = new List<Collider2D>();
			int num = Physics2D.OverlapCircle(transform.position, aoeSize, contactFilter, results);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					float random = Random.Range(1, 100);
					switch (results[i].gameObject.tag)
					{
						case "BlueEnemy":
							results[i].gameObject.GetComponent<EnemyBlue>().Knockback(impactForce, transform.position);
							if (random <= 10)
							{
								results[i].gameObject.GetComponent<EnemyBlue>().StartCoroutine("OverloadCoroutine", overloadDuration);
							}
							break;
						case "GreenEnemy":
							results[i].gameObject.GetComponent<EnemyGreen>().Knockback(impactForce, transform.position);
							if (random <= 10)
							{
								results[i].gameObject.GetComponent<EnemyGreen>().StartCoroutine("OverloadCoroutine", overloadDuration);
							}
							break;
						case "PurpleEnemy":
							results[i].gameObject.GetComponent<EnemyPurple>().Knockback(impactForce, transform.position);
							if (random <= 10)
							{
								results[i].gameObject.GetComponent<EnemyPurple>().StartCoroutine("OverloadCoroutine", overloadDuration);
							}
							break;
						case "RedEnemy":
							results[i].gameObject.GetComponent<EnemyRed>().Knockback(impactForce, transform.position);
							if (random <= 10)
							{
								results[i].gameObject.GetComponent<EnemyRed>().StartCoroutine("OverloadCoroutine", overloadDuration);
							}
							break;
						case "YellowEnemy":
							results[i].gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
							results[i].gameObject.GetComponent<EnemyYellow>().Knockback(impactForce, transform.position);
							if (random <= 10)
							{
								results[i].gameObject.GetComponent<EnemyYellow>().StartCoroutine("OverloadCoroutine", overloadDuration);
							}
							break;
					}
				}
			}
			exploded = true;
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


