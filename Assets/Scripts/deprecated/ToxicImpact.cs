using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicImpact : MonoBehaviour
{
	public static AudioManager audioManager;
	public GameObject player;
	public int projectileDamage;
	public int slowDuration;
	public float aoeSize;
	

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("ToxicImpact");
		player = GameObject.FindGameObjectWithTag("Player");

		aoeSize = player.GetComponent<Player>().toxicAoeSize;
		transform.localScale = new Vector3(aoeSize, aoeSize, 0);
		slowDuration = 4;
		
		projectileDamage = player.GetComponent<Player>().damageToxic;
		StartCoroutine(SelfDestruct());
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject.tag == "PurpleEnemy")
		{
			target.gameObject.GetComponent<EnemyPurple>().Slow(slowDuration);
		}
		else if (target.gameObject.tag == "BlueEnemy")
		{
			target.gameObject.GetComponent<EnemyBlue>().Slow(slowDuration);
		}
		else if (target.gameObject.tag == "GreenEnemy")
		{
			target.gameObject.GetComponent<EnemyGreen>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyGreen>().Slow(slowDuration);
		}
		else if (target.gameObject.tag == "RedEnemy")
		{
			target.gameObject.GetComponent<EnemyRed>().Slow(slowDuration);
		}
		else if (target.gameObject.tag == "YellowEnemy")
		{
			target.gameObject.GetComponent<EnemyYellow>().Slow(slowDuration);
		}
	}

	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
}
