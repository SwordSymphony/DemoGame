using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	public GameObject CanvasEnemy;
	public GameObject Enemy_bluePrefab;
	public GameObject Enemy_greenPrefab;
	public GameObject Enemy_redPrefab;
	public GameObject Enemy_yellowPrefab;
	public GameObject Enemy_purplePrefab;

	GameObject enemyPrefab;

	int maxEnemies;
	int currentEnemies;
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

	List<Vector2> spawnPoints = new List<Vector2>()
	{
		new Vector2(789, 300), new Vector2(781, 195), new Vector2(779, 96), new Vector2(627, 350), new Vector2(672, 277),
		new Vector2(651, 203), new Vector2(644, 116), new Vector2(685, 27), new Vector2(541, 296), new Vector2(553, 221),

		new Vector2(512, 157), new Vector2(513, 73), new Vector2(445, 339), new Vector2(444, 257), new Vector2(443, 109),
		new Vector2(384, 188), new Vector2(314, 294), new Vector2(327, 105), new Vector2(345, 27), new Vector2(229, 236),
		new Vector2(256, 178), new Vector2(202, 94)
	};

	void Start()
	{
		maxEnemies = 600;
		spawnDelay = 3.0f;
		spawnStartDelay = 2.0f;
		spawnAmount = 5;
		stage = 1;

		stageOneDuration = 120;   // = 300; 5 mins
		stageTwoDuration = 120;   // = 600;
		stageThreeDuration = 240; // = 900;

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
				isSpawning = true;
				InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay);
			}
		}
	}

	public IEnumerator ChangeStage()
	{
		// stage 0
		isSpawning = true;
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay);
		yield return new WaitForSeconds(stageOneDuration);

		// stage 1
		StopSpawn();
		spawnDelay /= 2;
		spawnAmount = 5;
		isSpawning = true;
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay); // start new spawn
		yield return new WaitForSeconds(stageTwoDuration);

		// stage 2
		StopSpawn();
		spawnDelay /= 2;
		spawnAmount = 7;
		stage = 2;
		isSpawning = true;
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay); // start new spawn
		yield return new WaitForSeconds(stageThreeDuration);

		// stage 3
		StopSpawn();
		spawnDelay /= 2;
		spawnAmount = 9;
		stage = 3;
		isSpawning = true;
		InvokeRepeating("SpawnMobs", spawnStartDelay, spawnDelay); // start new spawn
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
				_SpawnMobs(position);
				break;

			case 2:
				_SpawnMobs(position);
				_SpawnMobs(position);
				_SpawnMobs(position);
				break;
			
			case 3:
				_SpawnMobs(position);
				_SpawnMobs(position);
				_SpawnMobs(position);
				_SpawnMobs(position);
				break;
		}
	}

	public Vector2 GetSpawnPosition()
	{
		int random = Random.Range(0, 22);
		Vector2 spawnPosition = spawnPoints[random];
		return spawnPosition;
	}

	void _SpawnMobs(Vector2 position)
	{
		GetEnemyPrefab();
		int amount = Random.Range(3, spawnAmount);
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
		Debug.Log("spawn stopped");
		isSpawning = false;
	}
}