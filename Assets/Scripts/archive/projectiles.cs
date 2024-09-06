using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectiles : MonoBehaviour
{
	public int projectileDamage;

	void OnTriggerEnter2D(Collider2D target)
	{
		// if (target.gameObject.tag == "Enemy")
		// {
		// 	target.gameObject.GetComponent<Enemy>().TakeDamage(projectileDamage);
		// }

		if (target.gameObject.tag == "PurpleEnemy")
		{
			// target.gameObject.GetComponent<EnemyPurple>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyPurple>().StartCoroutine("FreezeMovement", 2);
		}
		else if(target.gameObject.tag == "BlueEnemy")
		{
			target.gameObject.GetComponent<EnemyBlue>().TakeDamage(projectileDamage);
			// target.gameObject.GetComponent<EnemyBlue>().freezeMovement(5);
			target.gameObject.GetComponent<EnemyBlue>().StartCoroutine("FreezeMovement", 2);
		}
		else if(target.gameObject.tag == "GreenEnemy")
		{
			// target.gameObject.GetComponent<EnemyGreen>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyGreen>().StartCoroutine("FreezeMovement", 2);
		}
		else if(target.gameObject.tag == "RedEnemy")
		{
			// target.gameObject.GetComponent<EnemyRed>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyRed>().StartCoroutine("FreezeMovement", 2);
		}
		else if(target.gameObject.tag == "YellowEnemy")
		{
			// target.gameObject.GetComponent<EnemyYellow>().TakeDamage(projectileDamage);
			target.gameObject.GetComponent<EnemyYellow>().StartCoroutine("FreezeMovement", 2);
		}
		Destroy(gameObject);
	}
}
