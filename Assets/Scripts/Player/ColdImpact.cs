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
		StartCoroutine(SelfDestruct());
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
						break;
					case "PurpleEnemy":
						results[i].gameObject.GetComponent<EnemyPurple>().Freeze(freezeDuration);
						break;
					case "RedEnemy":
						results[i].gameObject.GetComponent<EnemyRed>().Freeze(freezeDuration);
						break;
					case "YellowEnemy":
						results[i].gameObject.GetComponent<EnemyYellow>().Freeze(freezeDuration);
						break;
				}
			}
		}
		transform.localScale = new Vector3(impactSize, impactSize, 0);
	}

	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
}
