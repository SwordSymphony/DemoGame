using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkImpact : MonoBehaviour
{
	public static AudioManager audioManager;
	public GameObject player;
	public int projectileDamage;
	public float aoeSize;
	public int fearDuration;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("DarkImpact");
		player = GameObject.FindGameObjectWithTag("Player");

		aoeSize = player.GetComponent<Player>().darkAoeSize;
		transform.localScale = new Vector3(aoeSize, aoeSize, 0);
		fearDuration = 4;
		
		projectileDamage = player.GetComponent<Player>().damageDark;
		StartCoroutine(SelfDestruct());
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject.tag == "PurpleEnemy")
		{
			target.gameObject.GetComponent<EnemyPurple>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyPurple>().Fear(fearDuration);
		}
		else if (target.gameObject.tag == "BlueEnemy")
		{
			target.gameObject.GetComponent<EnemyBlue>().Fear(fearDuration);
		}
		else if (target.gameObject.tag == "GreenEnemy")
		{
			target.gameObject.GetComponent<EnemyGreen>().Fear(fearDuration);
		}
		else if (target.gameObject.tag == "RedEnemy")
		{
			target.gameObject.GetComponent<EnemyRed>().Fear(fearDuration);
		}
		else if (target.gameObject.tag == "YellowEnemy")
		{
			target.gameObject.GetComponent<EnemyYellow>().Fear(fearDuration);
		}
	}

	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
}