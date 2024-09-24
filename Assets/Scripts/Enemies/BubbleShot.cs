using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShot : MonoBehaviour
{
	public static AudioManager audioManager;
	public Animator animator;
	GameObject player;

	public int projectileDamage;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.PlayOneShot("FishWaterCubeShoot");
		player = GameObject.FindWithTag("Player");
		projectileDamage = 5;
	}
	void FixedUpdate()
	{
		transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 10.0f * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		// audioManager.PlayOneShot("DarkImpact");
		if (target.gameObject.tag == "Player")
		{
			target.gameObject.GetComponent<Player>().Freeze();
			target.gameObject.GetComponent<Player>().TakeDamage(projectileDamage, transform.position, 20, false);
		}
		animator.SetBool("exploded", true);
	}

	public void SelfDestruct()
	{
		Destroy(gameObject);
	}
}
