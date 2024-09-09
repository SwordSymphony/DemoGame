using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningExplosion : MonoBehaviour
{
	public static AudioManager audioManager;
	ContactFilter2D contactFilter;
	public int force;
	public float aoeSize;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("LightningImpact");
		aoeSize = 3.0f;
		force = 25;
		Explode();
	}

	void Explode()
	{
		List <Collider2D> results = new List<Collider2D>();
		int num = Physics2D.OverlapCircle(transform.position, aoeSize, contactFilter, results);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				switch (results[i].gameObject.tag)
				{
					case "GreenEnemy":
						results[i].gameObject.GetComponent<EnemyGreen>().StartCoroutine("KnockbackCoroutine", force);
						break;
					case "RedEnemy":
						results[i].gameObject.GetComponent<EnemyRed>().StartCoroutine("KnockbackCoroutine", force);
						break;
					case "PurpleEnemy":
						results[i].gameObject.GetComponent<EnemyPurple>().StartCoroutine("KnockbackCoroutine", force);
						break;
					case "BlueEnemy":
						results[i].gameObject.GetComponent<EnemyBlue>().StartCoroutine("KnockbackCoroutine", force);
						break;
				}
			}
		}
	}

	void SelfDestruct()
	{
		Destroy(gameObject);
	}
}
