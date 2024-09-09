using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicExplosion : MonoBehaviour
{
	public static AudioManager audioManager;
	ContactFilter2D contactFilter;
	public Animator animator;

	public int slowDuration;
	public float aoeSize;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("ToxicImpact");
		aoeSize = 3.0f;
		slowDuration = 4;
		Explode();
		StartCoroutine(SelfDestruct());
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
					case "BlueEnemy":
						results[i].gameObject.GetComponent<EnemyBlue>().Slow(slowDuration);
						break;
					case "RedEnemy":
						results[i].gameObject.GetComponent<EnemyRed>().Slow(slowDuration);
						break;
					case "PurpleEnemy":
						results[i].gameObject.GetComponent<EnemyPurple>().Slow(slowDuration);
						break;
					case "YellowEnemy":
						results[i].gameObject.GetComponent<EnemyYellow>().Slow(slowDuration);
						break;
				}
			}
		}
		animator.SetBool("exploded", true);
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		switch (target.gameObject.tag)
		{
			case "BlueEnemy":
				target.gameObject.GetComponent<EnemyBlue>().Slow(slowDuration);
				break;
			case "GreenEnemy":
				target.gameObject.GetComponent<EnemyGreen>().Slow(slowDuration);
				break;
			case "PurpleEnemy":
				target.gameObject.GetComponent<EnemyPurple>().Slow(slowDuration);
				break;
			case "RedEnemy":
				target.gameObject.GetComponent<EnemyRed>().Slow(slowDuration);
				break;
			case "YellowEnemy":
				target.gameObject.GetComponent<EnemyYellow>().Slow(slowDuration);
				break;
		}
	}

	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(4.0f);
		Destroy(gameObject);
	}
}
