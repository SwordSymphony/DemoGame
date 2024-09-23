using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyGreen : MonoBehaviour
{
	public GameObject Exp_orbPrefab;
	public GameObject LootPrefab;
	public GameObject RareLootPrefab;
	public GameObject ThrowingKnifePrefab;
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
	public LayerMask GreenEnemyLayer;
	public GameObject HealthBarGreenPrefab;

	public float attackRange;
	public float throwRange;
	bool playerInAttackRange;
	bool playerInThrowRange;
	float attackCooldownDuration;
	float throwCooldownDuration;
	float baseMoveSpeed;
	float currentMoveSpeed;
	float sprintCooldownDuration;
	float sprintDuration;
	int attackDamage;

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
	bool sprintCooldown;
	bool attackCooldown;
	bool isAttacking;
	bool throwCooldown;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioManager = FindObjectOfType<AudioManager>();
		rb = GetComponent<Rigidbody2D>();
		// player = GameObject.FindWithTag("Player");
		player = GameObject.FindGameObjectWithTag("Player");

		maxHealth = 100;
		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);

		onEnemyDie += Die; // subscription for method Die();
		contactFilter.SetLayerMask(GreenEnemyLayer);
		popRadius = 3.0f;
		baseMoveSpeed = 3.0f;
		currentMoveSpeed = baseMoveSpeed;
		sprintCooldownDuration = 10.0f;
		sprintDuration = 3.0f;
		attackRange = 4.0f;
		throwRange = 20.0f;
		attackCooldownDuration = 5.0f;
		throwCooldownDuration = 5.0f;

		attackDamage = 2;
		burnDamage = 2;
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

				if (!sprintCooldown)
				{
					// sprint
					StartCoroutine(SprintCoroutine());
				}

				playerInThrowRange = Physics2D.OverlapCircle(transform.position, throwRange, PlayerLayer);
				if (playerInThrowRange && !throwCooldown && !isAttacking)
				{
					StartCoroutine(ThrowKnife());
				}
				playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, PlayerLayer);
				if (playerInAttackRange && !attackCooldown)
				{
					RaycastHit2D target = Physics2D.Raycast(transform.position, player.transform.position);
					if (target && target.collider.tag == "Player")
					{
						StartCoroutine(Attack());
					}
				}
				transform.position = Vector2.MoveTowards(transform.position, player.transform.position, currentMoveSpeed * Time.deltaTime); // move to player
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
	public IEnumerator ThrowKnife()
	{
		Vector2 aim = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
		aim.Normalize();                                                                        // magnitude to 0.
		GameObject knife = Instantiate(ThrowingKnifePrefab, transform.position, Quaternion.identity);
		Rigidbody2D knifeRb = knife.GetComponent<Rigidbody2D>();

		knifeRb.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);
		knifeRb.AddForce(aim * 25, ForceMode2D.Impulse);

		throwCooldown = true;

		yield return new WaitForSeconds(throwCooldownDuration);
		throwCooldown = false;
	}

	// Attack
	public IEnumerator Attack()
	{
		isAttacking = true;
		Vector2 attackDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
		var angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
		// animator.SetBool("Attack", true);

		transform.GetChild(4).gameObject.SetActive(true);
		transform.GetChild(4).transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		// audioManager.PlayOneShot("MushroomAttack");
		yield return new WaitForSeconds(0.05f);

		transform.GetChild(4).gameObject.SetActive(false);
		yield return new WaitForSeconds(0.05f);

		transform.GetChild(4).gameObject.SetActive(true);
		// audioManager.PlayOneShot("MushroomAttack");
		yield return new WaitForSeconds(0.05f);

		transform.GetChild(4).gameObject.SetActive(false);
		yield return new WaitForSeconds(0.05f);

		transform.GetChild(4).gameObject.SetActive(true);
		
		yield return new WaitForSeconds(0.05f);

		// animator.SetBool("Attack", false);
		transform.GetChild(4).gameObject.SetActive(false);
		attackCooldown = true;
		isAttacking = false;

		yield return new WaitForSeconds(attackCooldownDuration);
		attackCooldown = false;
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject == player)
		{
			player.GetComponent<Player>().TakeDamage(attackDamage, transform.position, 4, false);
			audioManager.PlayOneShot("MushroomAttack");
		}
	}

	// Sprint
	public IEnumerator SprintCoroutine()
	{
		currentMoveSpeed = currentMoveSpeed + 5;
		sprintCooldown = true;
		yield return new WaitForSeconds(sprintDuration);
		currentMoveSpeed = baseMoveSpeed;
		yield return new WaitForSeconds(sprintCooldownDuration);
		sprintCooldown = false;
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
		currentMoveSpeed = currentMoveSpeed / 2;
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

	public void TakeBurnDamage()
	{
		if (!isHpBarActive)
		{
			GameObject health = Instantiate(HealthBarGreenPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity, transform);
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
			GameObject health = Instantiate(HealthBarGreenPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity, transform);
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
		GiveHealth();

		List <Collider2D> results = new List<Collider2D>();
		int num = Physics2D.OverlapCircle(transform.position, popRadius, contactFilter, results);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				var thisEnemy = results[i].gameObject.GetComponent<EnemyGreen>();
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
	
	public void GiveHealth()
	{
		player.GetComponent<Player>().Heal(1);
	}

	public void DropExp()
	{
		Instantiate(Exp_orbPrefab, transform.position, Quaternion.identity);
	}

	public void DropLoot()
	{
		GameObject instance = Instantiate(LootPrefab, transform.position, Quaternion.identity);
		instance.GetComponent<Loot>().type = 2;

		int roll = Random.Range(1, 101);
		if (roll == 100)
		{
			GameObject instanceRare = Instantiate(RareLootPrefab, transform.position, Quaternion.identity);
			instanceRare.GetComponent<Loot>().type = 2;
		}
	}

	// shuts of subscription to event, so its dont causing memory leak
	private void OnDisable()
	{
		onEnemyDie -= Die;
	}
}
