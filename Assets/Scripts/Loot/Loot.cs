using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
	GameObject player;
	LootProgress lootProgress;

	public float speed;
	public bool rare;
	public int type;
	bool playerInRadius;

	void Start()
	{
		player = GameObject.FindWithTag("Player");
		lootProgress = GameObject.FindWithTag("UI").GetComponent<LootProgress>();
	}

	void FixedUpdate()
	{
		if (playerInRadius)
		{
			transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
			if (transform.position == player.transform.position)
			{
				lootProgress.AddLoot(rare, type);
				Destroy(gameObject);
			}
		}
	}
	public void PlayerInRadius()
	{
		playerInRadius = true;
	}
}
