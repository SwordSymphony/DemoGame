using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyPurple : MonoBehaviour
{
	public GameObject Exp_orbPrefab;
	public GameObject LootPrefab;
	public GameObject RareLootPrefab;
	public GameObject LaserBeamPrefab;
	public GameObject OnDeathEffectPrefab;
	private SpriteRenderer spriteRenderer;
	public static AudioManager audioManager;
	public Rigidbody2D rb;

	public Animator animator;
	public Animator burnAnimator;
	public Animator freezeAnimator;
	public Animator fearAnimator;
	public Animator slowAnimator;

	public LayerMask PlayerLayer;
	public LayerMask PurpleEnemyLayer;
	public GameObject HealthBarPurplePrefab;

	public bool playerInAttackRange;
	public float shootRange;
	float baseMoveSpeed;
	float currentMoveSpeed;
	float ShootCooldownDuration;

	// Health
	public int maxHealth = 100;
	public int currentHealth;
	public HealthBar healthbar;

	public delegate void OnEnemyDie();
	public event OnEnemyDie onEnemyDie;
	public bool dieEventTriggered;

	public float popRadius;
	ContactFilter2D contactFilter;

	GameObject player;
	int burnDamage;

	bool isSlowed;
	bool isFrozen;
	bool isFeared;
	bool isBurning;
	bool isHpBarActive;
	bool shootCooldown;
	bool isAttacking;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioManager = FindObjectOfType<AudioManager>();
		rb = GetComponent<Rigidbody2D>();
		player = GameObject.FindWithTag("Player");

		maxHealth = 100;
		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);

		onEnemyDie += Die; // subscription for method Die();
		contactFilter.SetLayerMask(PurpleEnemyLayer);
		popRadius = 3.0f;
		baseMoveSpeed = 5.5f;
		currentMoveSpeed = baseMoveSpeed;
		burnDamage = 2;
		shootRange = 25.0f;
		ShootCooldownDuration = 6.0f;
		StartCoroutine(SpawnCoroutine());
	}

	// Update is called once per frame
	void Update()
	{
	}

	void FixedUpdate()
	{
		if (!isFrozen)
		{
			if (isFeared)
			{

				if (transform.position.x > player.transform.position.x)
				{
					spriteRenderer.flipX = true;
				}
				else
				{
					spriteRenderer.flipX = false;
				}

				Vector2 movementDirection = GetOppositeVector(1000);
				transform.position = Vector2.MoveTowards(transform.position, movementDirection, currentMoveSpeed * Time.deltaTime);
			}
			else
			{
				if (transform.position.x < player.transform.position.x)
				{
					spriteRenderer.flipX = true;
				}
				else
				{
					spriteRenderer.flipX = false;
				}

				bool playerInShootRange = Physics2D.OverlapCircle(transform.position, shootRange, PlayerLayer);
				if (playerInShootRange && !shootCooldown)
				{
					StartCoroutine(Laser());
				}
				transform.position = Vector2.MoveTowards(transform.position, player.transform.position, currentMoveSpeed * Time.deltaTime);
			}
			
		}
	}

	// spawn
	public IEnumerator SpawnCoroutine()
	{
		var collider = GetComponent<CapsuleCollider2D>();
		collider.enabled = false;
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
		isFrozen = true;
		// todo: animation for spawn

		yield return new WaitForSeconds(2.0f);
		collider.enabled = true;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		isFrozen = false;
	}

	// attack
	void OnCollisionEnter2D(Collision2D target)
	{
		if (target.gameObject == player)
		{
			target.gameObject.GetComponent<Player>().TakeDamage(3, transform.position, 0, false);
			audioManager.PlayOneShot("EyeBatBite");
		}
	}

	// shoot
	public IEnumerator Laser()
	{
		ShootLaserBeam();
		shootCooldown = true;
		yield return new WaitForSeconds(0.2f);
		ShootLaserBeam();
		yield return new WaitForSeconds(ShootCooldownDuration);
		shootCooldown = false;
	}

	public void ShootLaserBeam()
	{
		Vector2 aim = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
		aim.Normalize();
		GameObject laser = Instantiate(LaserBeamPrefab, transform.position, Quaternion.identity);
		Rigidbody2D laserRb = laser.GetComponent<Rigidbody2D>();

		laserRb.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);
		laserRb.AddForce(aim * 30, ForceMode2D.Impulse);
	}
	// Freeze
	public void Freeze(float freezeDuration)
	{
		if (isFrozen == true)                                      // if coroutine
		{
			StopCoroutine((FreezeCoroutine(freezeDuration)));      // stop it
		}
		StartCoroutine(FreezeCoroutine(freezeDuration));           // start new
	}

	public IEnumerator FreezeCoroutine(float seconds)
	{
		isFrozen = true;
		freezeAnimator.SetBool("Freeze", true);
		animator.SetBool("frozen", true);
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
		yield return new WaitForSeconds(seconds);
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;    // get freeze rotation back
		isFrozen = false;
		freezeAnimator.SetBool("Freeze", false);
		animator.SetBool("frozen", false);
	}

	// Slow
	public void Slow(int slowDuration)
	{
		if (isSlowed == true)                                      // if coroutine
		{
			StopCoroutine((SlowCoroutine(slowDuration)));          // stop it
		}
		StartCoroutine(SlowCoroutine(slowDuration));               // start new
	}

	public IEnumerator SlowCoroutine(int seconds)
	{
		isSlowed = true;
		currentMoveSpeed = baseMoveSpeed / 2;
		// TODO: slow attacks
		yield return new WaitForSeconds(seconds);
		isSlowed = false;
		currentMoveSpeed = baseMoveSpeed;
	}

	// Fear
	public void Fear(int fearDuration)
	{
		if (isFeared == true)                                      // if coroutine
		{
			StopCoroutine((FearCoroutine(fearDuration)));          // stop it
		}
		StartCoroutine(FearCoroutine(fearDuration));               // start new
	}

	public IEnumerator FearCoroutine(int seconds)
	{
		isFeared = true;
		currentMoveSpeed = baseMoveSpeed * 2;
		yield return new WaitForSeconds(seconds);
		isFeared = false;
		currentMoveSpeed = baseMoveSpeed;
	}

	// Burn
	public void Burn(int burnDuration)
	{
		if (isBurning == true)                                      // if coroutine
		{
			CancelInvoke("TakeBurnDamage");
			StopCoroutine((BurnCoroutine(burnDuration)));          // stop it
		}
		StartCoroutine(BurnCoroutine(burnDuration));               // start new
	}

	public IEnumerator BurnCoroutine(int seconds)
	{
		audioManager.PlayOneShot("EffectBurning");
		isBurning = true;
		burnAnimator.SetBool("Burn", true);
		InvokeRepeating("TakeBurnDamage", 0.0f, 0.2f);
		yield return new WaitForSeconds(seconds);
		CancelInvoke("TakeBurnDamage");
		isBurning = false;
		burnAnimator.SetBool("Burn", false);
		// audioManager.Stop("EffectBurning");
	}

	// Knockback
	public IEnumerator KnockbackCoroutine(int force)
	{
		Vector2 direction = GetOppositeVector(1);
		rb.AddForce(direction * force, ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.1f);
		rb.velocity = Vector3.zero;
	}

	public Vector2 GetOppositeVector(int multiplier)
	{
		float posX = transform.position.x;
		float posY = transform.position.y;

		if (player.transform.position.x >= transform.position.x)
		{
			posX = player.transform.position.x * - multiplier;
		}
		else if (player.transform.position.x <= transform.position.x)
		{
			posX = player.transform.position.x * multiplier;
		}

		if (player.transform.position.y >= transform.position.y)
		{
			posY = player.transform.position.y * - multiplier;
		}
		else if (player.transform.position.y <= transform.position.y)
		{
			posY = player.transform.position.y * multiplier;
		}
		Vector2 movementDirection = new Vector2(posX, posY);
		return movementDirection;
	}

	private void AttackPlayer()
	{
		// Debug.Log("attack");
	}

	public void TakeBurnDamage()
	{
		if (!isHpBarActive)
		{
			GameObject health = Instantiate(HealthBarPurplePrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity, transform);
			health.transform.localScale *= 3;
			var healthBarUI = health.GetComponent<HealthBar>(); // get script from instantiated health bar
			healthbar = healthBarUI; // Health bar from enemy connects to health bar UI.
			isHpBarActive = true;
		}
		currentHealth -= burnDamage;
		healthbar.SetHealth(currentHealth);

		if(currentHealth <= 0)
		{
			Die();
		}
	}

	public void TakeDamage(int damage)
	{
		if (!isHpBarActive)
		{
			GameObject health = Instantiate(HealthBarPurplePrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity, transform);
			health.transform.localScale *= 3;
			var healthBarUI = health.GetComponent<HealthBar>(); // get script from instantiated health bar
			healthbar = healthBarUI; // Health bar from enemy connects to health bar UI.
			isHpBarActive = true;
		}
		currentHealth -= damage;
		healthbar.SetHealth(currentHealth);
		audioManager.PlayOneShot("EyeBatDamage");

		if(currentHealth <= 0)
		{
			Die();
		}
	}

	public void Die()
	{
		Instantiate(OnDeathEffectPrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
		DropExp();
		DropLoot();

		List <Collider2D> results = new List<Collider2D>();
		int num = Physics2D.OverlapCircle(transform.position, popRadius, contactFilter, results);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				var thisEnemy = results[i].gameObject.GetComponent<EnemyPurple>();
				if (thisEnemy.onEnemyDie != null)
				{
					if (thisEnemy.dieEventTriggered == false)
					{
						thisEnemy.onEnemyDie();
						thisEnemy.dieEventTriggered = true;
					}
					
				}
			}
		}
	}

	public void DropExp()
	{
		Instantiate(Exp_orbPrefab, transform.position, Quaternion.identity);
	}

	public void DropLoot()
	{
		GameObject instance = Instantiate(LootPrefab, transform.position, Quaternion.identity);
		instance.GetComponent<Loot>().type = 3;

		int roll = Random.Range(1, 101);
		if (roll == 100)
		{
			GameObject instanceRare = Instantiate(RareLootPrefab, transform.position, Quaternion.identity);
			instanceRare.GetComponent<Loot>().type = 3;
		}
	}

	// shuts of subscription to event, so its dont causing memory leak
	private void OnDisable()
	{
		onEnemyDie -= Die;
	}
}
