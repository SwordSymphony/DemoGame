using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBlue : MonoBehaviour
{
	public GameObject player;
	public int projectileDamage;
	public int freezeDuration;
	public static AudioManager audioManager;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		// audioManager.PlayOneShot("ColdImpact");
		projectileDamage = player.GetComponent<Player>().damageCold;
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject.tag == "PurpleEnemy")
		{
			target.gameObject.GetComponent<EnemyPurple>().Freeze(freezeDuration);
		}
		else if (target.gameObject.tag == "BlueEnemy")
		{
			target.gameObject.GetComponent<EnemyBlue>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyBlue>().Freeze(freezeDuration);
		}
		else if (target.gameObject.tag == "GreenEnemy")
		{
			target.gameObject.GetComponent<EnemyGreen>().Freeze(freezeDuration);
		}
		else if (target.gameObject.tag == "RedEnemy")
		{
			target.gameObject.GetComponent<EnemyRed>().Freeze(freezeDuration);
		}
		else if (target.gameObject.tag == "YellowEnemy")
		{
			target.gameObject.GetComponent<EnemyYellow>().Freeze(freezeDuration);
		}
		Destroy(gameObject);
	}
}
