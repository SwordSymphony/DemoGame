using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningImpact : MonoBehaviour
{
	public static AudioManager audioManager;
	public GameObject player;
	public int projectileDamage;
	public int force;
	public float aoeSize;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("LightningImpact");
		player = GameObject.FindGameObjectWithTag("Player");

		aoeSize = player.GetComponent<Player>().lightningAoeSize;
		transform.localScale = new Vector3(aoeSize, aoeSize, 0);
		force = 15;
		
		projectileDamage = player.GetComponent<Player>().damageLightning;
		StartCoroutine(SelfDestruct());
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject.tag == "PurpleEnemy")
		{
			target.gameObject.GetComponent<EnemyPurple>().StartCoroutine("KnockbackCoroutine", force);
		}
		else if (target.gameObject.tag == "BlueEnemy")
		{
			target.gameObject.GetComponent<EnemyBlue>().StartCoroutine("KnockbackCoroutine", force);
		}
		else if (target.gameObject.tag == "GreenEnemy")
		{
			target.gameObject.GetComponent<EnemyGreen>().StartCoroutine("KnockbackCoroutine", force);
		}
		else if (target.gameObject.tag == "RedEnemy")
		{
			target.gameObject.GetComponent<EnemyRed>().StartCoroutine("KnockbackCoroutine", force);
		}
		else if (target.gameObject.tag == "YellowEnemy")
		{
			target.gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyYellow>().StartCoroutine("KnockbackCoroutine", force);
		}
	}

	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
}
