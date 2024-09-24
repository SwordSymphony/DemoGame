using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public GameObject shield;
	public GameObject CanvasUI;
	public Rigidbody2D rb;
	public Camera cam;

	public Animator animator;
	public Animator burnAnimator;
	public Animator freezeAnimator;
	public Animator slowAnimator;
	public GameObject heal;

	public static AudioManager audioManager;
	private SpriteRenderer spriteRenderer;
	
	// attacks and weapons
	public GameObject RotatePoint;
	public GameObject ShootPoint1;
	public GameObject ShootPoint2;
	public GameObject ShootPoint3;
	public GameObject ShootPoint4;
	public GameObject ShootPoint5;
	public GameObject WeaponsBarPrefab;
	GameObject weaponType;
	
	public GameObject WeaponBluePrefab;   // weapon against blue enemies
	public GameObject WeaponGreenPrefab;  // weapon against green enemies
	public GameObject WeaponPurplePrefab; // weapon against purple enemies
	public GameObject WeaponRedPrefab;    // weapon against red enemies
	public GameObject WeaponYellowPrefab; // weapon against yellow enemies

	// damage
	public int damageCold;
	public int damageToxic;
	public int damageDark;
	public int damageFire;
	public int damageLightning;
	// overlap circle of aoe
	public float coldAoeSize;
	public float toxicAoeSize;
	public float darkAoeSize;
	public float fireAoeSize;
	public float lightningAoeSize;
	// sprite size
	public float coldImpactSize;
	public float toxicImpactSize;
	public float darkImpactSize;
	public float fireImpactSize;
	public float lightningImpactSize;
	// attack speed multiplier
	public float attackSpeedCold;
	public float attackSpeedToxic;
	public float attackSpeedDark;
	public float attackSpeedFire;
	public float attackSpeedLightning;
	// bullet force
	public float bulletForceToxic;
	public float bulletForceDark;
	public float bulletForceFire;
	public float bulletForceLightning;

	float invulnerabilityDuration;

	float attackSpeed;

	float lastAttackTime;
	int projectilesAmount;
	public List<int> projectiles = new List<int>(){1,1,1,1,1};

	public float dashCooldown;

	// ray
	LayerMask mask;
	public GameObject frostRayStartPrefab;
	public GameObject frostRayImpactPrefab;
	GameObject frostImpactInstance;
	GameObject frostStartInstance;
	bool rayActive;

	public LineRenderer lineRenderer;
	public LineRenderer lineRenderer2;
	public LineRenderer lineRenderer3;

	public float bulletForce;
	public float moveSpeed;

	// hp
	public HealthBar healthbar;
	public int maxHealth;
	public int currentHealth;
	// shield
	public int shieldAmount;
	public float shieldTimer;

	Vector2 movement;
	Vector2 mousePos;
	Vector2 aim;
	Vector2 aim2;
	Vector2 aim3;
	Vector2 aim4;
	Vector2 aim5;

	Color defaultColor;

	// status
	bool knockback;
	bool frostSoundIsPlaying;
	bool isDashing;
	bool isDashOnCooldown;
	bool invulnerable;
	bool isBurning;
	bool isFrozen;

	void Start ()
	{
		// CanvasUI = GameObject.FindWithTag("UI");
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioManager = FindObjectOfType<AudioManager>();
		rb = GetComponent<Rigidbody2D>(); 
		mask = LayerMask.GetMask("Ignore Raycast");

		defaultColor = spriteRenderer.color;

		// health
		maxHealth = 100;
		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);

		bulletForce = 35;
		moveSpeed = 15;
		projectilesAmount = 1;

		invulnerabilityDuration = 1.0f;
		dashCooldown = 1.0f;
		shieldTimer = 10.0f;

		// current weapon
		weaponType = WeaponBluePrefab;
		selectWeapon(0);
		projectilesAmount = projectiles[0];
		attackSpeed = attackSpeedCold;
		animator.SetInteger("weaponType", 0);

		// load player data
		LoadProgress();
	}

	void LoadProgress()
	{
		PlayerProgressData data = SaveSystem.LoadPlayerProgress();

		if (data is not null && data.damageCold > 0)
		{
			// damage
			damageCold = data.damageCold;
			damageToxic = data.damageToxic;
			damageDark = data.damageDark;
			damageFire = data.damageFire;
			damageLightning = data.damageLightning;

			// overlap circle of aoe
			coldAoeSize = data.coldAoeSize;
			toxicAoeSize = data.toxicAoeSize;
			darkAoeSize = data.darkAoeSize;
			fireAoeSize = data.fireAoeSize;
			lightningAoeSize = data.lightningAoeSize;

			// sprite size
			coldImpactSize = data.coldImpactSize;
			toxicImpactSize = data.toxicImpactSize;
			darkImpactSize = data.darkImpactSize;
			fireImpactSize = data.fireImpactSize;
			lightningImpactSize = data.lightningImpactSize;

			// cooldown
			attackSpeedCold = data.attackSpeedCold;
			attackSpeedToxic = data.attackSpeedToxic;
			attackSpeedDark = data.attackSpeedDark;
			attackSpeedFire = data.attackSpeedFire;
			attackSpeedLightning = data.attackSpeedLightning;

			projectiles = data.projectiles;
		}
	}

	// Update is called once per frame
	void Update()
	{
		ChangeWeaponType();

		movement.x = Input.GetAxisRaw("Horizontal"); // x input for movement
		movement.y = Input.GetAxisRaw("Vertical");   // y input for movement

		if (movement.sqrMagnitude > 0)
		{
			animator.SetBool("isMoving", true);
		}
		if (movement.sqrMagnitude <= 0)
		{
			animator.SetBool("isMoving", false);
		}

		// mouse position
		mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetKeyDown("space"))
		{
			// Dash
			if(!isDashOnCooldown && !isFrozen)
			{
				if (movement.x != 0 || movement.y != 0)
				{
					StartCoroutine(Dash());
				}
			}
		}
		
		if (Input.GetButton("Fire1") && !isFrozen)
		{
			animator.SetBool("Attack", true);
			animator.SetFloat("AttackSpeed", attackSpeed);

			if (weaponType == WeaponBluePrefab)
			{
				if (!frostSoundIsPlaying)
				{
					audioManager.Play("ColdCast", true);
					frostSoundIsPlaying = true;
				}
				// StartCoroutine(ShootRay());
				ShootRay();
			}
		}
		else if (Input.GetButtonUp("Fire1"))
		{
			Destroy(frostImpactInstance);
			Destroy(frostStartInstance);
			lineRenderer.enabled = false;
			rayActive = false;

			animator.SetBool("Attack", false);
			animator.SetFloat("AttackSpeed", 1.0f);
			audioManager.Stop("ColdCast");
			frostSoundIsPlaying = false;
		}
	}

	void Shoot()
	{
		switch (projectilesAmount)
			{
				case 1:
					ShootOneProjectile();
					break;
				case 2:
					ShootTwoProjectiles(true);
					break;
				case 3:
					ShootOneProjectile();
					Shoot3And4Projectiles();
					break;
				case 4:
					ShootFourProjectiles();
					break;
				case 5:
					ShootFiveProjectiles();
					break;
			}
	}

	void FixedUpdate()
	{
		if (!knockback && !isDashing)
		{
			rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);  // movement
		}

		aim = mousePos - rb.position;
		aim2 = ShootPoint2.transform.position - RotatePoint.transform.position;
		aim3 = ShootPoint3.transform.position - RotatePoint.transform.position;
		aim4 = ShootPoint4.transform.position - RotatePoint.transform.position;
		aim5 = ShootPoint5.transform.position - RotatePoint.transform.position;

		var angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
		RotatePoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		if (aim.x > 0)
		{
			spriteRenderer.flipX = true;
		}
		else
		{
			spriteRenderer.flipX = false;
		}
	}

	IEnumerator Dash()
	{
		isDashOnCooldown = true;
		isDashing = true;

		gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
		gameObject.GetComponent<CircleCollider2D>().enabled = true;
		int force = 3000;
		rb.AddForce(movement * force, ForceMode2D.Impulse);
		audioManager.PlayOneShot("Dash");
		yield return new WaitForSeconds(0.3f);

		rb.velocity = Vector3.zero;
		isDashing = false;
		gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
		gameObject.GetComponent<CircleCollider2D>().enabled = false;

		yield return new WaitForSeconds(dashCooldown);
		isDashOnCooldown = false;
	}

	void ChangeWeaponType()
	{
		if (Input.GetKey(KeyCode.Alpha1))
		{
			if (weaponType != WeaponBluePrefab)
			{
				weaponType = WeaponBluePrefab;
				selectWeapon(0);
				projectilesAmount = projectiles[0];
				attackSpeed = attackSpeedCold;
				animator.SetInteger("weaponType", 0);
			}
		}
		else if (Input.GetKey(KeyCode.Alpha2))
		{
			if (weaponType != WeaponGreenPrefab)
			{
				weaponType = WeaponGreenPrefab;
				projectilesAmount = projectiles[1];
				attackSpeed = attackSpeedToxic;
				bulletForce = bulletForceToxic;
				selectWeapon(1);
				animator.SetInteger("weaponType", 1);
			}
		}
		else if (Input.GetKey(KeyCode.Alpha3))
		{
			if (weaponType != WeaponPurplePrefab)
			{
				weaponType = WeaponPurplePrefab;
				projectilesAmount = projectiles[2];
				attackSpeed = attackSpeedDark;
				bulletForce = bulletForceDark;
				selectWeapon(2);
				animator.SetInteger("weaponType", 2);
			}
		}
		else if (Input.GetKey(KeyCode.Alpha4))
		{
			if (weaponType != WeaponRedPrefab)
			{
				weaponType = WeaponRedPrefab;
				projectilesAmount = projectiles[3];
				attackSpeed = attackSpeedFire;
				bulletForce = bulletForceFire;
				selectWeapon(3);
				animator.SetInteger("weaponType", 3);
			}
		}
		else if (Input.GetKey(KeyCode.Alpha5))
		{
			if (weaponType != WeaponYellowPrefab)
			{
				weaponType = WeaponYellowPrefab;
				projectilesAmount = projectiles[4];
				attackSpeed = attackSpeedLightning;
				bulletForce = bulletForceLightning;
				selectWeapon(4);
				animator.SetInteger("weaponType", 4);
			}
		}
	}

	void selectWeapon(int selected)
	{
		for (int i = 0; i <= 4; i++)
		{
			if (i == selected)
			{
				WeaponsBarPrefab.gameObject.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
			}
			else
			{
				WeaponsBarPrefab.gameObject.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = false;
			}
		}
		
	}

	public void Heal(int amount)
	{
		if (currentHealth + amount >= maxHealth)
		{
			currentHealth = maxHealth;
			healthbar.SetHealth(maxHealth);
			StartCoroutine(HealCoroutine());
		}
		else
		{
			currentHealth += amount;
			healthbar.SetHealth(currentHealth);
			StartCoroutine(HealCoroutine());
		}
	}
	
	public IEnumerator HealCoroutine()
	{
		heal.SetActive(true);
		yield return new WaitForSeconds(0.3f);
		heal.SetActive(false);
	}

	public void GetShield(int amount)
	{
		if (shieldAmount > 0)
		{
			StopCoroutine(ShieldCoroutine());
		}
		shieldAmount += amount;
		StartCoroutine(ShieldCoroutine());
		shield.SetActive(true);
		// animator shield true
	}

	public IEnumerator ShieldCoroutine()
	{
		yield return new WaitForSeconds(shieldTimer);
		shieldAmount = 0;
		shield.SetActive(false);
		// animator shield false
	}

	public void TakeDamage(int damage, Vector2 hitDirection, int knockbackForce, bool ignoreInvulnerability)
	{
		if (!invulnerable || ignoreInvulnerability)
		{
			if (shieldAmount > 0)
			{
				if (shieldAmount >= damage)
				{
					shieldAmount -= damage;
					if (shieldAmount == 0)
					{
						// shield false
						shield.SetActive(false);
					}
					return;
				}
				else if (shieldAmount < damage)
				{
					damage -= shieldAmount;
					shieldAmount = 0;
					// shield false
					shield.SetActive(false);
				}
			}
			currentHealth -= damage;
			healthbar.SetHealth(currentHealth);

			int force = 100 * knockbackForce;
			if (force != 0)
			{
				StartCoroutine(KnockbackCoroutine(force, hitDirection));
			}

			if (currentHealth <= 0)
			{
				Time.timeScale = 0f;
				audioManager.Stop("SagaOfTheSeaWolves");
				CanvasUI.GetComponent<LootProgress>().PlayerDeath();
				CanvasUI.GetComponent<PauseMenu>().gameOver = true;
				// todo: game over sound?
				// save player data
			}
			StartCoroutine(invulnerabilityCoroutine());
			if (!isFrozen)
			{
				StartCoroutine(BlinkCoroutine());
			}
		}
	}

	void TakeBurnDamage()
	{
		currentHealth -= 1;
		healthbar.SetHealth(currentHealth);

		if (currentHealth <= 0)
		{
			Time.timeScale = 0f;
			audioManager.Stop("SagaOfTheSeaWolves");
			CanvasUI.GetComponent<LootProgress>().PlayerDeath();
			CanvasUI.GetComponent<PauseMenu>().gameOver = true;
			// todo: game over sound?
		}
	}

	public IEnumerator invulnerabilityCoroutine()
	{
		invulnerable = true;
		yield return new WaitForSeconds(invulnerabilityDuration);
		invulnerable = false;
	}

	private IEnumerator BlinkCoroutine() {
		Color blinkColor = new Color(1, 1, 1, 0.5f);

		spriteRenderer.color = blinkColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = defaultColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = blinkColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = defaultColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = blinkColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = defaultColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = blinkColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = defaultColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = blinkColor;
		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = defaultColor;
	}

	// Knockback
	public IEnumerator KnockbackCoroutine(int force, Vector2 hitDirection)
	{
		knockback = true;
		Vector2 knockbackDirection = rb.position - hitDirection;
		rb.AddForce(knockbackDirection.normalized * force, ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.3f);
		rb.velocity = Vector3.zero;
		knockback = false;
	}

	// Freeze
	public void Freeze()
	{
		if (isFrozen == true)                                      // if coroutine
		{
			return;
		}
		StartCoroutine(FreezeCoroutine(1));           // start new
	}

	public IEnumerator FreezeCoroutine(float seconds)
	{
		Color freezeColor = new Color(1, 1, 1, 0.7f);

		isFrozen = true;
		StopCoroutine(BlinkCoroutine());
		spriteRenderer.color = freezeColor;
		freezeAnimator.SetBool("Freeze", true);
		rb.constraints = RigidbodyConstraints2D.FreezeAll;

		yield return new WaitForSeconds(seconds);
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;    // get freeze rotation back
		isFrozen = false;
		spriteRenderer.color = defaultColor;
		freezeAnimator.SetBool("Freeze", false);
	}

	// // Slow
	// public void Slow(int slowDuration)
	// {
	// 	if (isSlowed == true)                                      // if coroutine
	// 	{
	// 		StopCoroutine((SlowCoroutine(slowDuration)));          // stop it
	// 	}
	// 	StartCoroutine(SlowCoroutine(slowDuration));               // start new
	// }

	// public IEnumerator SlowCoroutine(int seconds)
	// {
	// 	isSlowed = true;
	// 	currentMoveSpeed = baseMoveSpeed / 2;
	// 	yield return new WaitForSeconds(seconds);
	// 	isSlowed = false;
	// 	currentMoveSpeed = baseMoveSpeed;
	// }

	// Burn
	public void Burn()
	{
		if (isBurning == true)                                      // if coroutine
		{
			CancelInvoke("TakeBurnDamage");
			StopCoroutine((BurnCoroutine()));          // stop it
		}
		StartCoroutine(BurnCoroutine());               // start new
	}

	public IEnumerator BurnCoroutine()
	{
		// audioManager.PlayOneShot("EffectBurning");
		isBurning = true;
		burnAnimator.SetTrigger("Burn");
		InvokeRepeating("TakeBurnDamage", 0.0f, 0.2f);
		yield return new WaitForSeconds(2);
		CancelInvoke("TakeBurnDamage");
		isBurning = false;
		burnAnimator.ResetTrigger("Burn");
	}

	// IEnumerator ShootRay()
	// {
	// 	Vector3 direction = new Vector3(mousePos.x - ShootPoint1.transform.position.x, mousePos.y - ShootPoint1.transform.position.y, 0);
	// 	float distance = Vector2.Distance(mousePos, ShootPoint1.transform.position);
	// 	RaycastHit2D target = Physics2D.Raycast(ShootPoint1.transform.position, direction, distance, ~mask); // ray to direction
	// 	Vector3 impactPoint = mousePos; // default impact point to mouse

	// 	if (target)
	// 	{
	// 		impactPoint = target.transform.position; // impact point if ray collides
	// 	}
	// 	lineRenderer.SetPosition(0, ShootPoint1.transform.position); // line from start point
	// 	lineRenderer.SetPosition(1, impactPoint);                       // line to end point

	// 	GameObject instance = Instantiate(frostRayImpactPrefab, impactPoint, Quaternion.identity); // instantiate prefab to end point
	// 	instance.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);
	// 	GameObject instanceStart = Instantiate(frostRayStartPrefab, ShootPoint1.transform.position, Quaternion.identity);
	// 	instanceStart.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);

	// 	lineRenderer.enabled = true;
	// 	yield return new WaitForSeconds(0.6f);
	// 	lineRenderer.enabled = false;
	// }

	void ShootRay()
	{
		Vector3 direction = new Vector3(mousePos.x - ShootPoint1.transform.position.x, mousePos.y - ShootPoint1.transform.position.y, 0);
		float distance = Vector2.Distance(mousePos, ShootPoint1.transform.position);
		RaycastHit2D target = Physics2D.Raycast(ShootPoint1.transform.position, direction, distance, ~mask); // ray to direction
		Vector3 impactPoint = mousePos; // default impact point to mouse

		if (target)
		{
			impactPoint = target.transform.position; // impact point if ray collides
		}
		lineRenderer.SetPosition(0, ShootPoint1.transform.position); // line from start point
		lineRenderer.SetPosition(1, impactPoint);                       // line to end point

		if (!rayActive) // if no impact instance
		{
			frostImpactInstance = Instantiate(frostRayImpactPrefab, impactPoint, Quaternion.identity); // instantiate prefab to end point
			frostImpactInstance.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);

			frostStartInstance = Instantiate(frostRayStartPrefab, ShootPoint1.transform.position, Quaternion.identity);
			frostStartInstance.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);
			rayActive = true;
		}
		else // if already exist
		{
			// move it
			frostImpactInstance.transform.position = impactPoint;
			frostImpactInstance.transform.rotation = Quaternion.identity;
			frostImpactInstance.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);

			frostStartInstance.transform.position = ShootPoint1.transform.position;
			frostStartInstance.transform.rotation = Quaternion.identity;
			frostStartInstance.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);
		}
		lineRenderer.enabled = true;
	}

	// shoot 1 straight
	void ShootOneProjectile()
	{
		aim.Normalize();                                                                        // magnitude to 0.
		GameObject bullet = Instantiate(weaponType, ShootPoint1.transform.position, Quaternion.identity);   // instantiate bullet.
		Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();                              // Get RB.

		rbBullet.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);       // Rotate bullet.
		rbBullet.AddForce(aim * bulletForce, ForceMode2D.Impulse);                              // Add force to bullet.

		// crutch for now
		if (weaponType == WeaponPurplePrefab && aim.x < 0)
		{
			bullet.GetComponent<SpriteRenderer>().flipY = true;
			bullet.GetComponent<WeaponPurple>().isFlipped = true;
		}
	}

	// shoot 2 straight
	void ShootTwoProjectiles(bool straight)
	{
		Vector2 shootingAim4 = aim4;
		Vector2 shootingAim5 = aim5;
		if (straight)
		{
			shootingAim4 = aim;
			shootingAim5 = aim;
		}
		shootingAim4.Normalize();
		shootingAim5.Normalize();
		GameObject bullet4 = Instantiate(weaponType, ShootPoint4.transform.position, Quaternion.identity);   // instantiate bullet.
		Rigidbody2D rbBullet4 = bullet4.GetComponent<Rigidbody2D>();                              // Get RB.

		GameObject bullet5 = Instantiate(weaponType, ShootPoint5.transform.position, Quaternion.identity);   // instantiate bullet.
		Rigidbody2D rbBullet5 = bullet5.GetComponent<Rigidbody2D>();                              // Get RB.

		rbBullet4.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(shootingAim4.y, shootingAim4.x) * Mathf.Rad2Deg);       // Rotate bullet.
		rbBullet4.AddForce(shootingAim4 * bulletForce, ForceMode2D.Impulse);                              // Add force to bullet.

		rbBullet5.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(shootingAim5.y, shootingAim5.x) * Mathf.Rad2Deg);       // Rotate bullet.
		rbBullet5.AddForce(shootingAim5 * bulletForce, ForceMode2D.Impulse);                              // Add force to bullet.

		// crutch for now
		if (weaponType == WeaponPurplePrefab && aim.x < 0)
		{
			bullet4.GetComponent<SpriteRenderer>().flipY = true;
			bullet5.GetComponent<SpriteRenderer>().flipY = true;
			bullet4.GetComponent<WeaponPurple>().isFlipped = true;
			bullet5.GetComponent<WeaponPurple>().isFlipped = true;
		}
	}

	// shoot 2 in a cone 
	void Shoot3And4Projectiles()
	{
		aim2.Normalize();
		aim3.Normalize();

		GameObject bullet2 = Instantiate(weaponType, ShootPoint2.transform.position, Quaternion.identity);   // instantiate bullet.
		GameObject bullet3 = Instantiate(weaponType, ShootPoint3.transform.position, Quaternion.identity);   // instantiate bullet.
		
		Rigidbody2D rbBullet2 = bullet2.GetComponent<Rigidbody2D>();                              // Get RB.
		Rigidbody2D rbBullet3 = bullet3.GetComponent<Rigidbody2D>();                              // Get RB.

		rbBullet2.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim2.y, aim2.x) * Mathf.Rad2Deg);       // Rotate bullet.
		rbBullet2.AddForce(aim2 * bulletForce, ForceMode2D.Impulse);                              // Add force to bullet.

		rbBullet3.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim3.y, aim3.x) * Mathf.Rad2Deg);       // Rotate bullet.
		rbBullet3.AddForce(aim3 * bulletForce, ForceMode2D.Impulse);                              // Add force to bullet.

		// crutch for now
		if (weaponType == WeaponPurplePrefab && aim.x < 0)
		{
			bullet2.GetComponent<SpriteRenderer>().flipY = true;
			bullet3.GetComponent<SpriteRenderer>().flipY = true;
			bullet2.GetComponent<WeaponPurple>().isFlipped = true;
			bullet3.GetComponent<WeaponPurple>().isFlipped = true;
		}
	}

	// shoot 2 straight and 2 in a cone
	void ShootFourProjectiles()
	{
		ShootTwoProjectiles(true);
		Shoot3And4Projectiles();
	}

	// shoot 1 straight and 4 in a cone
	void ShootFiveProjectiles()
	{
		ShootOneProjectile();
		ShootTwoProjectiles(false);
		Shoot3And4Projectiles();
	}
}
