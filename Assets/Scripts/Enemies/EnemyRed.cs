using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyRed : MonoBehaviour
{
	public GameObject Exp_orbPrefab;
	public GameObject LootPrefab;
	public GameObject RareLootPrefab;
	public GameObject OnDeathEffectPrefab;
	private SpriteRenderer spriteRenderer;
	public static AudioManager audioManager;
	public Rigidbody2D rb;

	public Animator animator;
	public Animator burnAnimator;
	public Animator freezeAnimator;
	public Animator fearAnimator;
	public Animator slowAnimator;
	public Animator overloadAnimator;

	public LayerMask PlayerLayer;
	public LayerMask RedEnemyLayer;
	public GameObject HealthBarRedPrefab;

	public float attackRange;
	public float shootRange;
	float attackCooldownDuration;
	float shootCooldownDuration;
	float baseMoveSpeed;
	float currentMoveSpeed;

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
	bool attackCooldown;
	bool ShootCooldown;
	bool isBiting;
	bool isFireBreathing;
	public bool isOverload;

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
		contactFilter.SetLayerMask(RedEnemyLayer);
		popRadius = 3.0f;
		baseMoveSpeed = 2.5f;
		currentMoveSpeed = baseMoveSpeed;
		burnDamage = 2;
		attackRange = 2.0f;
		shootRange = 9.0f;
		attackCooldownDuration = 4.0f;
		shootCooldownDuration = 8.0f;
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
				if (playerInShootRange && !ShootCooldown && !isBiting)
				{
					StartCoroutine(FireBreath());
				}

				bool playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, PlayerLayer);
				if (playerInAttackRange && !attackCooldown && !isFireBreathing)
				{
					StartCoroutine(BiteAttack());
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
		animator.SetBool("isMoving", false);
		// todo: animation for spawn

		yield return new WaitForSeconds(2.0f);
		collider.enabled = true;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		isFrozen = false;
		animator.SetBool("isMoving", true);
		animator.SetBool("spawned", true);
	}

	// shoot
	public IEnumerator FireBreath()
	{
		isFireBreathing = true;

		Vector2 attackDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
		var angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
		// animator.SetBool("Attack", true);

		transform.GetChild(6).gameObject.SetActive(true);
		transform.GetChild(6).transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		yield return new WaitForSeconds(2.0f);

		isFireBreathing = false;
		ShootCooldown = true;
		// animator.SetBool("RamAttack", false);
		transform.GetChild(6).gameObject.SetActive(false);
		yield return new WaitForSeconds(shootCooldownDuration);

		ShootCooldown = false;
	}
	// attack
	public IEnumerator BiteAttack()
	{
		isBiting = true;

		Vector2 attackDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
		var angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
		// animator.SetBool("Attack", true);

		transform.GetChild(5).gameObject.SetActive(true);
		transform.GetChild(5).transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		yield return new WaitForSeconds(0.3f);

		isBiting = false;
		attackCooldown = true;
		// animator.SetBool("RamAttack", false);
		transform.GetChild(5).gameObject.SetActive(false);
		yield return new WaitForSeconds(attackCooldownDuration);

		attackCooldown = false;
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject == player && isBiting)
		{
			target.gameObject.GetComponent<Player>().TakeDamage(10, transform.position, 30, false);
		}
	}

	void OnTriggerStay2D(Collider2D target)
	{
		if (target.gameObject == player && isFireBreathing)
		{
			target.gameObject.GetComponent<Player>().TakeDamage(4, transform.position, 0, false);
			target.gameObject.GetComponent<Player>().Burn();
		}
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
		animator.SetBool("isMoving", false);
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
		yield return new WaitForSeconds(seconds);
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;    // get freeze rotation back
		isFrozen = false;
		freezeAnimator.SetBool("Freeze", false);
		animator.SetBool("isMoving", true);
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
		fearAnimator.SetBool("Fear", true);
		currentMoveSpeed = baseMoveSpeed * 2;
		yield return new WaitForSeconds(seconds);
		isFeared = false;
		fearAnimator.SetBool("Fear", false);
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
		InvokeRepeating("TakeBurnDamage", 0.2f, 0.2f);
		yield return new WaitForSeconds(seconds);
		CancelInvoke("TakeBurnDamage");
		isBurning = false;
		burnAnimator.SetBool("Burn", false);
		// audioManager.Stop("EffectBurning");
	}
	// overload
	public IEnumerator OverloadCoroutine(int seconds)
	{
		isOverload = true;
		overloadAnimator.SetBool("overload", true);
		yield return new WaitForSeconds(seconds);
		isOverload = false;
		overloadAnimator.SetBool("overload", false);
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
		else if (player.transform.position.x == transform.position.x)
		{
			posX = 0;
		}

		if (player.transform.position.y >= transform.position.y)
		{
			posY = player.transform.position.y * - multiplier;
		}
		else if (player.transform.position.y <= transform.position.y)
		{
			posY = player.transform.position.y * multiplier;
		}
		else if (player.transform.position.y == transform.position.y)
		{
			posY = 0;
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
			GameObject health = Instantiate(HealthBarRedPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity, transform);
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
			GameObject health = Instantiate(HealthBarRedPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity, transform);
			health.transform.localScale *= 3;
			var healthBarUI = health.GetComponent<HealthBar>(); // get script from instantiated health bar
			healthbar = healthBarUI; // Health bar from enemy connects to health bar UI.
			isHpBarActive = true;
		}
		currentHealth -= damage;
		healthbar.SetHealth(currentHealth);

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
				var thisEnemy = results[i].gameObject.GetComponent<EnemyRed>();
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
		instance.GetComponent<Loot>().type = 4;

		int roll = Random.Range(1, 101);
		if (roll == 100)
		{
			GameObject instanceRare = Instantiate(RareLootPrefab, transform.position, Quaternion.identity);
			instanceRare.GetComponent<Loot>().type = 4;
		}
	}

	// shuts of subscription to event, so its dont causing memory leak
	private void OnDisable()
	{
		onEnemyDie -= Die;
	}
}
