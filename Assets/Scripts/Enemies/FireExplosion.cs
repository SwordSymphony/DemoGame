using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : MonoBehaviour
{
	public Animator animator;
	public int burnDuration;
	public float aoeSize;
	public static AudioManager audioManager;
	ContactFilter2D contactFilter;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("FireImpact");
		aoeSize = 3.0f;
		burnDuration = 4;
		Explode();
		StartCoroutine(SelfDestruct());
	}

	// for explosion
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
						results[i].gameObject.GetComponent<EnemyBlue>().Burn(burnDuration);
						break;
					case "GreenEnemy":
						results[i].gameObject.GetComponent<EnemyGreen>().Burn(burnDuration);
						break;
					case "PurpleEnemy":
						results[i].gameObject.GetComponent<EnemyPurple>().Burn(burnDuration);
						break;
					case "YellowEnemy":
						results[i].gameObject.GetComponent<EnemyYellow>().Burn(burnDuration);
						break;
				}
			}
		}
		animator.SetBool("exploded", true);
	}

	// for burning ground.
	void OnTriggerEnter2D(Collider2D target)
	{
		switch (target.gameObject.tag)
		{
			case "BlueEnemy":
				target.gameObject.GetComponent<EnemyBlue>().Burn(burnDuration);
				break;
			case "GreenEnemy":
				target.gameObject.GetComponent<EnemyGreen>().Burn(burnDuration);
				break;
			case "PurpleEnemy":
				target.gameObject.GetComponent<EnemyPurple>().Burn(burnDuration);
				break;
			case "RedEnemy":
				target.gameObject.GetComponent<EnemyRed>().Burn(burnDuration);
				break;
			case "YellowEnemy":
				target.gameObject.GetComponent<EnemyYellow>().Burn(burnDuration);
				break;
			case "Player":
				target.gameObject.GetComponent<Player>().Burn();
				break;
		}
	}
	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(4.0f);
		Destroy(gameObject);
	}
}
