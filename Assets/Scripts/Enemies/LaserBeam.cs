using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
	public int projectileDamage;
	public static AudioManager audioManager;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("EyeBatBeam");
		projectileDamage = 5;
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject.tag == "Player")
		{
			target.gameObject.GetComponent<Player>().TakeDamage(projectileDamage, transform.position, 0, false);
		}
		Destroy(gameObject);
	}
}
