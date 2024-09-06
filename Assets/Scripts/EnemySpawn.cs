using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawn : MonoBehaviour
{
	public GameObject SpawnZone;
	public Tilemap tileMap;
	public GameObject player;
	public GameObject CanvasEnemy;
	public GameObject Enemy_bluePrefab;
	public GameObject Enemy_greenPrefab;
	public GameObject Enemy_redPrefab;
	public GameObject Enemy_yellowPrefab;
	public GameObject Enemy_purplePrefab;

	GameObject enemyPrefab;

	// public delegate void OnEnemyDie();
	// public event OnEnemyDie onEnemyDie;

	public int maxEnemies;
	public int currentEnemies;
	int enemyDeath;
	float spawnDelay;
	float spawnStartDelay;
	int spawnAmount;

	public float currentTime;
	float stageOneDuration;
	float stageTwoDuration;
	float stageThreeDuration;
	int stage;

	bool isSpawning;

	void Start()
	{
		maxEnemies = 500;
		spawnDelay = 3.0f;
		spawnStartDelay = 2.0f;
		spawnAmount = 2;
		stage = 1;

		stageOneDuration = 60;   // = 300; 5 mins
		stageTwoDuration = 60;   // = 600;
		stageThreeDuration = 60; // = 900;

		SpawnMobs();

		StartCoroutine(ChangeStage());
	}

	void Update()
	{
	}

	void FixedUpdate()
	{
		if (!isSpawning)
		{
			if (currentEnemies < maxEnemies)
			{
				InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay);
				isSpawning = true;
			}
		}
		// todo:
		// if (currentEnemies < lowEnemiesAmount)
		// {
		// 	// increase spawn rate
		// }
	}

	public IEnumerator ChangeStage()
	{
		// stage 0
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay);
		isSpawning = true;
		yield return new WaitForSeconds(stageOneDuration);

		// stage 1
		StopSpawn();
		spawnDelay /= 2;
		spawnAmount = 4;
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay); // start new spawn
		isSpawning = true;
		yield return new WaitForSeconds(stageTwoDuration);

		// stage 2
		StopSpawn();
		spawnDelay /= 2;
		spawnAmount = 6;
		stage = 2;
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay); // start new spawn
		isSpawning = true;
		yield return new WaitForSeconds(stageThreeDuration);

		// stage 3
		StopSpawn();
		spawnDelay /= 2;
		spawnAmount = 8;
		stage = 3;
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay); // start new spawn
		isSpawning = true;
	}

	public void EnemyDeathCounter()
	{
		enemyDeath++;
		currentEnemies--;

		// todo: total deaths tracker.
	}

	public void GetEnemyPrefab()
	{
		int color = Random.Range(1, 6);

		if (color == 1)
		{
			enemyPrefab = Enemy_bluePrefab;
		}
		else if (color == 2)
		{
			enemyPrefab = Enemy_greenPrefab;
		}
		else if (color == 3)
		{
			enemyPrefab = Enemy_purplePrefab;
		}
		else if (color == 4)
		{
			enemyPrefab = Enemy_redPrefab;
		}
		else 
		{
			enemyPrefab = Enemy_yellowPrefab;
		}
	}

	public void SpawnMobs()
	{
		Vector2 position = GetSpawnPosition();
		switch (stage)
		{
			case 1:

				_SpawnMobs(position);
				break;

			case 2:
				_SpawnMobs(position);
				_SpawnMobs(position);
				break;
			
			case 3:
				_SpawnMobs(position);
				_SpawnMobs(position);
				_SpawnMobs(position);
				break;
		}
	}

	public Vector2 GetSpawnPosition()
	{
		var spawnCollider = SpawnZone.GetComponent<BoxCollider2D>();
		var spawnCollider1 = SpawnZone.GetComponent<PolygonCollider2D>();

		float colliderBoundsXmin = spawnCollider.bounds.min.x;
		float colliderBoundsXmax = spawnCollider.bounds.max.x;
		float colliderBoundsYmin = spawnCollider.bounds.min.y;
		float colliderBoundsYmax = spawnCollider.bounds.max.y;

		int random = Random.Range(0, 2);
		if (random != 0)
		{
			colliderBoundsXmin = spawnCollider1.bounds.min.x;
			colliderBoundsXmax = spawnCollider1.bounds.max.x;
			colliderBoundsYmin = spawnCollider1.bounds.min.y;
			colliderBoundsYmax = spawnCollider1.bounds.max.y;
		}
		var xPos = Random.Range(colliderBoundsXmin, colliderBoundsXmax);
		var yPos = Random.Range(colliderBoundsYmin, colliderBoundsYmax);

		Vector2 spawnPosition = new Vector2(xPos, yPos);
		return spawnPosition;
	}

	void _SpawnMobs(Vector2 position)
	{
		GetEnemyPrefab();
		int amount = Random.Range(2, spawnAmount);
		for (int i = 0; i < amount; i++)
		{
			GameObject instance = Instantiate(enemyPrefab, position, transform.rotation, CanvasEnemy.transform);
			var offset = new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
			position += offset;

			if (enemyPrefab == Enemy_bluePrefab)
			{
				instance.GetComponent<EnemyBlue>().onEnemyDie += EnemyDeathCounter;
			}
			else if (enemyPrefab == Enemy_greenPrefab)
			{
				instance.GetComponent<EnemyGreen>().onEnemyDie += EnemyDeathCounter;
			}
			else if (enemyPrefab == Enemy_purplePrefab)
			{
				instance.GetComponent<EnemyPurple>().onEnemyDie += EnemyDeathCounter;
			}
			else if (enemyPrefab == Enemy_redPrefab)
			{
				instance.GetComponent<EnemyRed>().onEnemyDie += EnemyDeathCounter;
			}
			else if (enemyPrefab == Enemy_yellowPrefab)
			{
				instance.GetComponent<EnemyYellow>().onEnemyDie += EnemyDeathCounter;
			}
		}
		currentEnemies += amount;

		if (currentEnemies >= maxEnemies)
		{
			StopSpawn();
		}
	}

	void StopSpawn()
	{
		CancelInvoke("SpawnMobs");
		isSpawning = false;
	}
}