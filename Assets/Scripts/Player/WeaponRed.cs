using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRed : MonoBehaviour
{
	public static AudioManager audioManager;
	public Rigidbody2D rb;
	public Animator animator;
	public GameObject player;

	public int projectileDamage;
	public int burnDuration;
	public int burnDamage;
	public float aoeSize;
	public float impactSize;
	
	ContactFilter2D contactFilter;
	// public LayerMask PlayerLayer;

	bool triggered;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("FireCast");
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>(); 

		aoeSize = player.GetComponent<Player>().fireAoeSize;
		impactSize = player.GetComponent<Player>().fireImpactSize;
		projectileDamage = player.GetComponent<Player>().damageFire;

		burnDuration = 4;
		burnDamage= 4;
		// contactFilter = ~PlayerLayer;
		Destroy(gameObject, 1);
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
							results[i].gameObject.GetComponent<EnemyBlue>().Burn(burnDuration);
							if (results[i].gameObject.GetComponent<EnemyBlue>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyBlue>().TakeDamage(projectileDamage);
							}
							break;
						case "GreenEnemy":
							results[i].gameObject.GetComponent<EnemyGreen>().Burn(burnDuration);
							if (results[i].gameObject.GetComponent<EnemyGreen>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyGreen>().TakeDamage(projectileDamage);
							}
							break;
						case "PurpleEnemy":
							results[i].gameObject.GetComponent<EnemyPurple>().Burn(burnDuration);
							if (results[i].gameObject.GetComponent<EnemyPurple>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyPurple>().TakeDamage(projectileDamage);
							}
							break;
						case "RedEnemy":
							results[i].gameObject.GetComponent<EnemyRed>().TakeDamage(projectileDamage);
							results[i].gameObject.GetComponent<EnemyRed>().Burn(burnDuration);
							break;
						case "YellowEnemy":
							results[i].gameObject.GetComponent<EnemyYellow>().Burn(burnDuration);
							if (results[i].gameObject.GetComponent<EnemyYellow>().isOverload == true)
							{
								results[i].gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
							}
							break;
					}
				}
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
