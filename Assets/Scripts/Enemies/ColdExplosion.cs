using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdExplosion : MonoBehaviour
{
	public static AudioManager audioManager;
	ContactFilter2D contactFilter;
	public Animator animator;

	public int freezeDuration;
	public float aoeSize;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("ToxicImpact");
		aoeSize = 3.0f;
		freezeDuration = 4;
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
					case "GreenEnemy":
						results[i].gameObject.GetComponent<EnemyGreen>().Freeze(freezeDuration);
						break;
					case "RedEnemy":
						results[i].gameObject.GetComponent<EnemyRed>().Freeze(freezeDuration);
						break;
					case "PurpleEnemy":
						results[i].gameObject.GetComponent<EnemyPurple>().Freeze(freezeDuration);
						break;
					case "YellowEnemy":
						results[i].gameObject.GetComponent<EnemyYellow>().Freeze(freezeDuration);
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
				target.gameObject.GetComponent<EnemyBlue>().Freeze(freezeDuration);
				break;
			case "GreenEnemy":
				target.gameObject.GetComponent<EnemyGreen>().Freeze(freezeDuration);
				break;
			case "PurpleEnemy":
				target.gameObject.GetComponent<EnemyPurple>().Freeze(freezeDuration);
				break;
			case "RedEnemy":
				target.gameObject.GetComponent<EnemyRed>().Freeze(freezeDuration);
				break;
			case "YellowEnemy":
				target.gameObject.GetComponent<EnemyYellow>().Freeze(freezeDuration);
				break;
			case "Player":
				target.gameObject.GetComponent<Player>().Freeze();
				break;
		}
	}

	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(4.0f);
		Destroy(gameObject);
	}
}
