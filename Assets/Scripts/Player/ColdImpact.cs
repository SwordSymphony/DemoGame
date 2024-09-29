using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdImpact : MonoBehaviour
{
	public static AudioManager audioManager;
	public Rigidbody2D rb;
	public Animator animator;
	public GameObject player;
	public int rayDamage;
	public float aoeSize;
	public float impactSize;
	public float freezeDuration;

	ContactFilter2D contactFilter;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		player = GameObject.FindGameObjectWithTag("Player");

		aoeSize = player.GetComponent<Player>().coldAoeSize;
		impactSize = player.GetComponent<Player>().coldImpactSize;
		rayDamage = player.GetComponent<Player>().damageCold;
		freezeDuration = 2.5f;

		inflictDamage();
		InvokeRepeating("inflictDamage", 0.0f, 0.1f);
	}

	void inflictDamage()
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
						results[i].gameObject.GetComponent<EnemyBlue>().TakeDamage(rayDamage);
						results[i].gameObject.GetComponent<EnemyBlue>().Freeze(freezeDuration);
						break;
					case "GreenEnemy":
						results[i].gameObject.GetComponent<EnemyGreen>().Freeze(freezeDuration);
						if (results[i].gameObject.GetComponent<EnemyGreen>().isOverload == true)
						{
							results[i].gameObject.GetComponent<EnemyGreen>().TakeDamage(rayDamage);
						}
						break;
					case "PurpleEnemy":
						results[i].gameObject.GetComponent<EnemyPurple>().Freeze(freezeDuration);
						if (results[i].gameObject.GetComponent<EnemyPurple>().isOverload == true)
						{
							results[i].gameObject.GetComponent<EnemyPurple>().TakeDamage(rayDamage);
						}
						break;
					case "RedEnemy":
						results[i].gameObject.GetComponent<EnemyRed>().Freeze(freezeDuration);
						if (results[i].gameObject.GetComponent<EnemyRed>().isOverload == true)
						{
							results[i].gameObject.GetComponent<EnemyRed>().TakeDamage(rayDamage);
						}
						break;
					case "YellowEnemy":
						results[i].gameObject.GetComponent<EnemyYellow>().Freeze(freezeDuration);
						if (results[i].gameObject.GetComponent<EnemyYellow>().isOverload == true)
						{
							results[i].gameObject.GetComponent<EnemyYellow>().TakeDamage(rayDamage);
						}
						break;
				}
			}
		}
		transform.localScale = new Vector3(impactSize, impactSize, 0);
	}

	public void SelfDestruct()
	{
		Destroy(gameObject);
	}
}
