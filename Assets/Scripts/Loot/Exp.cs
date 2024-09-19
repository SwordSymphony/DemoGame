using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
	LootProgress lootProgress;
	GameObject player;
	public float speed;

	void Start()
	{
		player = GameObject.FindWithTag("Player");
		lootProgress = GameObject.FindWithTag("UI").GetComponent<LootProgress>();
	}

	void FixedUpdate()
	{
		transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
		if (transform.position == player.transform.position)
		{
			lootProgress.AddExp();
			Destroy(gameObject);
		}
	}
}
