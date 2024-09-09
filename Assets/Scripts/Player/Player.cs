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
	public Animator childAnimator;
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
	public float fireImpactSize;
	public float coldImpactSize;
	public float toxicImpactSize;
	public float darkImpactSize;
	public float lightningImpactSize;

	public float cooldownCold;
	public float cooldownToxic;
	public float cooldownDark;
	public float cooldownFire;
	public float cooldownLightning;
	public float invulnerabilityDuration;

	public float projectileCooldown;
	public float rayCooldown;
	float attackCooldown;
	float lastAttackTime;
	int projectilesAmount;
	public List<int> projectiles = new List<int>(){1,1,1,1,1};

	public float dashCooldown;

	// ray
	LayerMask mask;
	public GameObject ColdImpactPrefab;
	public LineRenderer lineRenderer;

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

	// status
	bool knockback;
	bool frostSoundIsPlaying;
	bool isDashing;
	bool isDashOnCooldown;
	bool invulnerable;
	bool isBurning;

	void Start ()
	{
		CanvasUI = GameObject.FindWithTag("UI");
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioManager = FindObjectOfType<AudioManager>();
		rb = GetComponent<Rigidbody2D>(); 
		// mask = LayerMask.GetMask("PlayerLayer");
		mask = LayerMask.GetMask("Ignore Raycast");

		maxHealth = 100;
		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);

		weaponType = WeaponBluePrefab;
		selectWeapon(0);

		projectileCooldown = 0.45f;
		rayCooldown = 0.15f;
		bulletForce = 35;
		moveSpeed = 15;
		projectilesAmount = 1;

		damageCold = 3;
		damageToxic = 25;
		damageDark = 25;
		damageFire = 25;
		damageLightning = 25;

		// overlap circle of aoe
		coldAoeSize = 1.75f;
		toxicAoeSize = 3.5f;
		darkAoeSize = 3.5f;
		fireAoeSize = 3.5f;
		lightningAoeSize = 3.5f;

		// sprite size
		coldImpactSize = 0.5f;
		toxicImpactSize = 1.0f;
		darkImpactSize = 1.0f;
		fireImpactSize = 1.0f;
		lightningImpactSize = 1.0f;

		// pause between attacks.
		cooldownCold = 0.08f;
		cooldownToxic = 0.45f;
		cooldownDark = 0.45f;
		cooldownFire = 0.45f;
		cooldownLightning = 0.45f;
		invulnerabilityDuration = 1.0f;

		dashCooldown = 1.0f;

		shieldTimer = 10.0f;
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
			if(!isDashOnCooldown)
			{
				if (movement.x != 0 || movement.y != 0)
				{
					StartCoroutine(Dash());
				}
			}
		}

		if (Input.GetButton("Fire1"))
		{
			animator.SetBool("Attack", true);
			if (Time.time > lastAttackTime + attackCooldown)   // if now cooldown
			{
				if (weaponType == WeaponBluePrefab)
				{
					if (!frostSoundIsPlaying)
					{
						audioManager.Play("ColdCast", true);
						frostSoundIsPlaying = true;
					}
					StartCoroutine(ShootRay());
					attackCooldown = rayCooldown;           // set cooldown
					lastAttackTime = Time.time;                    // get time of last attack
				}
				else
				{
					if (projectilesAmount == 1)
					{
						ShootOneProjectile();
					}
					else if (projectilesAmount == 2)
					{
						ShootTwoProjectiles(true);
					}
					else if (projectilesAmount == 3)
					{
						ShootOneProjectile();
						Shoot3And4Projectiles();
					}
					else if (projectilesAmount == 4)
					{
						ShootFourProjectiles();
					}
					else if (projectilesAmount == 5)
					{
						ShootFiveProjectiles();
					}
					attackCooldown = projectileCooldown;           // set cooldown
					lastAttackTime = Time.time;                    // get time of last attack
				}
			}
		}
		else if (Input.GetButtonUp("Fire1"))
		{
			animator.SetBool("Attack", false);
			audioManager.Stop("ColdCast");
			frostSoundIsPlaying = false;
		}
	}

	void FixedUpdate()
	{
		if (!knockback && !isDashing)
		{
			rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);  // movement
		}

		Vector3 shootDirection2 = ShootPoint2.transform.position - RotatePoint.transform.position;
		Vector3 shootDirection3 = ShootPoint3.transform.position - RotatePoint.transform.position;
		Vector3 shootDirection4 = ShootPoint4.transform.position - RotatePoint.transform.position;
		Vector3 shootDirection5 = ShootPoint5.transform.position - RotatePoint.transform.position;
		// var angle1 = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
		aim = mousePos - rb.position;
		aim2 = shootDirection2;
		aim3 = shootDirection3;
		aim4 = shootDirection4;
		aim5 = shootDirection5;
		
		// animator.SetFloat("MousePosX", aim.x);
		// animator.SetFloat("MousePosY", aim.y);
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

		gameObject.GetComponent<CircleCollider2D>().enabled = false;
		int force = 3000;
		rb.AddForce(movement * force, ForceMode2D.Impulse);
		audioManager.PlayOneShot("Dash");
		yield return new WaitForSeconds(0.3f);

		rb.velocity = Vector3.zero;
		isDashing = false;
		gameObject.GetComponent<CircleCollider2D>().enabled = true;

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
				// WeaponsBarPrefab.gameObject.GetComponent<Image>().enabled = false; // it works!
				// WeaponsBarPrefab.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false; // it works too!
				selectWeapon(0);
				projectilesAmount = projectiles[0];
				rayCooldown = cooldownCold;
				animator.SetInteger("weaponType", 0);
			}
		}
		else if (Input.GetKey(KeyCode.Alpha2))
		{
			if (weaponType != WeaponGreenPrefab)
			{
				weaponType = WeaponGreenPrefab;
				projectilesAmount = projectiles[1];
				attackCooldown = cooldownToxic;
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
				attackCooldown = cooldownDark;
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
				attackCooldown = cooldownFire;
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
				attackCooldown = cooldownLightning;
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
		}
		else
		{
			currentHealth += amount;
			healthbar.SetHealth(currentHealth);
		}
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
				CanvasUI.GetComponent<GameProgress>().PlayerDeath();
				CanvasUI.GetComponent<PauseMenu>().gameOver = true;
				// todo: game over sound?
			}
			StartCoroutine(invulnerabilityCoroutine());
			StartCoroutine(BlinkCoroutine());
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
			CanvasUI.GetComponent<GameProgress>().PlayerDeath();
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
		Color defaultColor = spriteRenderer.color;
		Color blinkColor = new Color(1, 1, 1,1);

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
		childAnimator.SetTrigger("Burn");
		InvokeRepeating("TakeBurnDamage", 0.0f, 0.2f);
		yield return new WaitForSeconds(2);
		CancelInvoke("TakeBurnDamage");
		isBurning = false;
		childAnimator.ResetTrigger("Burn");
	}

	IEnumerator ShootRay()
	{
		Vector3 rayToPoint = new Vector3(aim.x, aim.y, 0);

		RaycastHit2D target = Physics2D.Raycast(ShootPoint1.transform.position, rayToPoint, 20.0f, ~mask);
		Vector2 instancePoint = new Vector2();
		instancePoint = transform.position + rayToPoint;

		if (target)
		{
			instancePoint = target.point;
		}
		lineRenderer.SetPosition(0, ShootPoint1.transform.position);
		lineRenderer.SetPosition(1, instancePoint);

		GameObject instance = Instantiate(ColdImpactPrefab, instancePoint, Quaternion.identity);
		instance.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);

		lineRenderer.enabled = true;
		yield return new WaitForSeconds(0.5f);
		lineRenderer.enabled = false;
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
