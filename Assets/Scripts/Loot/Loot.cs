using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
	GameObject player;
	GameProgress gameProgress;

	public float speed;
	public bool rare;
	public int type;

	void Start()
	{
		player = GameObject.FindWithTag("Player");
		gameProgress = GameObject.FindWithTag("UI").GetComponent<GameProgress>();
	}

	void FixedUpdate()
	{
		transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
		if (transform.position == player.transform.position)
		{
			gameProgress.AddLoot(rare, type);
			Destroy(gameObject);
		}
	}
}
