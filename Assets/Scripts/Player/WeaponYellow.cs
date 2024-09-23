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
	
	ContactFilter2D contactFilter;
	// public LayerMask PlayerLayer;

	bool triggered;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("LightningCast");
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>(); 

		aoeSize = player.GetComponent<Player>().toxicAoeSize;
		impactSize = player.GetComponent<Player>().toxicImpactSize;
		projectileDamage = player.GetComponent<Player>().damageToxic;

		force = 15;
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
							results[i].gameObject.GetComponent<EnemyBlue>().StartCoroutine("KnockbackCoroutine", force);
							break;
						case "GreenEnemy":
							results[i].gameObject.GetComponent<EnemyGreen>().StartCoroutine("KnockbackCoroutine", force);
							break;
						case "PurpleEnemy":
							results[i].gameObject.GetComponent<EnemyPurple>().StartCoroutine("KnockbackCoroutine", force);
							break;
						case "RedEnemy":
							results[i].gameObject.GetComponent<EnemyRed>().StartCoroutine("KnockbackCoroutine", force);
							break;
						case "YellowEnemy":
							results[i].gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
							results[i].gameObject.GetComponent<EnemyYellow>().StartCoroutine("KnockbackCoroutine", force);
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


