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

	bool exploded;
	float penetrationCount;
	float penetrationCountMax;

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
		penetrationCountMax = 3;

		Destroy(gameObject, 1);
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (penetrationCount < penetrationCountMax) // penetrate
		{
			switch (target.gameObject.tag)
				{
					case "BlueEnemy":
						target.gameObject.GetComponent<EnemyBlue>().StartCoroutine("KnockbackCoroutine", force);
						break;
					case "GreenEnemy":
						target.gameObject.GetComponent<EnemyGreen>().StartCoroutine("KnockbackCoroutine", force);
						break;
					case "PurpleEnemy":
						target.gameObject.GetComponent<EnemyPurple>().StartCoroutine("KnockbackCoroutine", force);
						break;
					case "RedEnemy":
						target.gameObject.GetComponent<EnemyRed>().StartCoroutine("KnockbackCoroutine", force);
						break;
					case "YellowEnemy":
						target.gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
						target.gameObject.GetComponent<EnemyYellow>().StartCoroutine("KnockbackCoroutine", force);
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


