using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkExplosion : MonoBehaviour
{
	public Animator animator;
	int fearDuration;
	public float aoeSize;
	public static AudioManager audioManager;
	ContactFilter2D contactFilter;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("DarkImpact");
		aoeSize = 3.0f;
		fearDuration = 4;
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
						results[i].gameObject.GetComponent<EnemyBlue>().Fear(fearDuration);
						break;
					case "GreenEnemy":
						results[i].gameObject.GetComponent<EnemyGreen>().Fear(fearDuration);
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
		animator.SetBool("exploded", true);
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		switch (target.gameObject.tag)
		{
			case "BlueEnemy":
				target.gameObject.GetComponent<EnemyBlue>().Fear(fearDuration);
				break;
			case "GreenEnemy":
				target.gameObject.GetComponent<EnemyGreen>().Fear(fearDuration);
				break;
			case "PurpleEnemy":
				target.gameObject.GetComponent<EnemyPurple>().Fear(fearDuration);
				break;
			case "RedEnemy":
				target.gameObject.GetComponent<EnemyRed>().Fear(fearDuration);
				break;
			case "YellowEnemy":
				target.gameObject.GetComponent<EnemyYellow>().Fear(fearDuration);
				break;
		}
	}
	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(4.0f);
		Destroy(gameObject);
	}
}
