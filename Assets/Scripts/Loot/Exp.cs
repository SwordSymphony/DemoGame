using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
	GameProgress gameProgress;
	GameObject player;
	public float speed;

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
			gameProgress.AddExp();
			Destroy(gameObject);
		}
	}
}
