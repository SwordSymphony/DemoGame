using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPurple : MonoBehaviour
{
	public static AudioManager audioManager;
	public Rigidbody2D rb;
	public Animator animator;
	public GameObject player;
	public int projectileDamage;
	public float aoeSize;
	public float impactSize;
	public int fearDuration;

	public bool isFlipped;
	
	ContactFilter2D contactFilter;
	// public LayerMask PlayerLayer;

	bool triggered;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("DarkCast");
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>(); 

		aoeSize = player.GetComponent<Player>().darkAoeSize;
		impactSize = player.GetComponent<Player>().darkImpactSize;
		projectileDamage = player.GetComponent<Player>().damageDark;

		fearDuration = 4;
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
							results[i].gameObject.GetComponent<EnemyBlue>().Fear(fearDuration);
							break;
						case "GreenEnemy":
							results[i].gameObject.GetComponent<EnemyGreen>().Fear(fearDuration);
							break;
						case "PurpleEnemy":
							results[i].gameObject.GetComponent<EnemyPurple>().TakeDamage(projectileDamage);
							results[i].gameObject.GetComponent<EnemyPurple>().Fear(fearDuration);
							break;
						case "RedEnemy":
							results[i].gameObject.GetComponent<EnemyRed>().Fear(fearDuration);
							break;
						case "YellowEnemy":
							results[i].gameObject.GetComponent<EnemyYellow>().Fear(fearDuration);
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
