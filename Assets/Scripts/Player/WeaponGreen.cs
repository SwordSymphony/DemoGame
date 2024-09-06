using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGreen : MonoBehaviour
{
	public static AudioManager audioManager;
	public Rigidbody2D rb;
	public Animator animator;
	public GameObject player;
	public int projectileDamage;
	public float aoeSize;
	public float impactSize;
	public int slowDuration;
	
	ContactFilter2D contactFilter;
	// public LayerMask PlayerLayer;

	bool triggered;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("ToxicCast");
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>(); 

		aoeSize = player.GetComponent<Player>().toxicAoeSize;
		impactSize = player.GetComponent<Player>().toxicImpactSize;
		projectileDamage = player.GetComponent<Player>().damageToxic;

		slowDuration = 4;
		// contactFilter = ~PlayerLayer;
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
							results[i].gameObject.GetComponent<EnemyBlue>().Slow(slowDuration);
							break;
						case "GreenEnemy":
							results[i].gameObject.GetComponent<EnemyGreen>().TakeDamage(projectileDamage);
							results[i].gameObject.GetComponent<EnemyGreen>().Slow(slowDuration);
							break;
						case "PurpleEnemy":
							results[i].gameObject.GetComponent<EnemyPurple>().Slow(slowDuration);
							break;
						case "RedEnemy":
							results[i].gameObject.GetComponent<EnemyRed>().Slow(slowDuration);
							break;
						case "YellowEnemy":
							results[i].gameObject.GetComponent<EnemyYellow>().Slow(slowDuration);
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

