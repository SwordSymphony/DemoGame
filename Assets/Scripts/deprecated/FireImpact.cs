using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireImpact : MonoBehaviour
{
	public GameObject player;
	public int projectileDamage;
	public int burnDuration;
	public int burnDamage;
	public float aoeSize;
	public static AudioManager audioManager;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("FireImpact");
		player = GameObject.FindGameObjectWithTag("Player");

		aoeSize = player.GetComponent<Player>().fireAoeSize;
		transform.localScale = new Vector3(aoeSize, aoeSize, 0);
		
		projectileDamage = player.GetComponent<Player>().damageFire;
		StartCoroutine(SelfDestruct());
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject.tag == "PurpleEnemy")
		{
			target.gameObject.GetComponent<EnemyPurple>().Burn(burnDuration);
		}
		else if (target.gameObject.tag == "BlueEnemy")
		{
			target.gameObject.GetComponent<EnemyBlue>().Burn(burnDuration);
		}
		else if (target.gameObject.tag == "GreenEnemy")
		{
			target.gameObject.GetComponent<EnemyGreen>().Burn(burnDuration);
		}
		else if (target.gameObject.tag == "RedEnemy")
		{
			target.gameObject.GetComponent<EnemyRed>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyRed>().Burn(burnDuration);
		}
		else if (target.gameObject.tag == "YellowEnemy")
		{
			target.gameObject.GetComponent<EnemyYellow>().Burn(burnDuration);
		}
	}

	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
}
