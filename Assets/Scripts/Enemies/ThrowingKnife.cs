using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
	public int projectileDamage;
	public static AudioManager audioManager;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("MushroomShoot");
		projectileDamage = 5;
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		audioManager.PlayOneShot("GetHit");
		if (target.gameObject.tag == "Player")
		{
			target.gameObject.GetComponent<Player>().TakeDamage(projectileDamage, transform.position, 2, false);
		}
		Destroy(gameObject);
	}
}
