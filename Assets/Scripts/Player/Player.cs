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

	private AnimatorStateInfo currentStateInfo;
	private float currentAnimationTime;

	private DirectionState currentDirectionState;
	private bool movementDirectionChanged;

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
	int weaponSelected;
	
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

	int projectilesAmount;
	public List<int> projectiles = new List<int>(){1,1,1,1,1};

	public float dashCooldown;

	// ray
	LayerMask mask;
	public GameObject frostRayStartPrefab;
	public GameObject frostRayImpactPrefab;
	GameObject frostImpactInstance;
	GameObject frostImpactInstance2;
	GameObject frostImpactInstance3;
	GameObject frostStartInstance;
	bool rayActive;
	bool secondRayActive;
	public LineRenderer lineRenderer;
	public LineRenderer lineRenderer2;
	public LineRenderer lineRenderer3;

	public float bulletForce;
	public float moveSpeed;
	public float pickUpRadius;
	LayerMask lootLayerMask;

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
	bool spread;
	bool isChangingWeapon;

	private bool isShootingCooldown; // To track if the cooldown is active
	private float shootingCooldownDuration; // Duration of the cooldown

	void Start ()
	{
		// CanvasUI = GameObject.FindWithTag("UI");
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioManager = FindObjectOfType<AudioManager>();
		rb = GetComponent<Rigidbody2D>(); 
		mask = LayerMask.GetMask("Ignore Raycast");
		lootLayerMask = LayerMask.GetMask("Loot");

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
		weaponSelected = 0;
		projectilesAmount = projectiles[0];
		attackSpeed = attackSpeedCold;
		animator.SetInteger("weaponType", 0);

		shootingCooldownDuration = 0.05f;

		// load player data
		LoadProgress();
	}

	private enum DirectionState
	{
		Right,
		UpRight,
		Up,
		UpLeft,
		Left,
		DownLeft,
		Down,
		DownRight
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
		ChangeWeaponTypeMouse();

		if (!isChangingWeapon && !isFrozen)
		{
			Movement();
		}

		// mouse position
		mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

		// Dash skill
		if (Input.GetKeyDown("space"))
		{
			if(!isDashOnCooldown && !isFrozen)
			{
				if (movement.x != 0 || movement.y != 0)
				{
					StartCoroutine(Dash());
				}
			}
		}
		
		// attack/shoot
		if (Input.GetButton("Fire1") && !isFrozen && !isChangingWeapon)
		{
			// if currently attacking
			if (animator.GetBool("Attack") == true)
			{
				// Get current animation state info
				currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
				currentAnimationTime = currentStateInfo.normalizedTime;
				// Handle direction change for attack
				if (movementDirectionChanged)
				{
					// Determine the new attack state based on the current aim direction
					string newAttackState = DetermineAttackState();
					// Play the new attack animation at the current animation time
					animator.Play(newAttackState, 0, currentAnimationTime);
					movementDirectionChanged = false;
				}
			}

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
				ShootAdditionalRays();
			}
		}
		// if stop attacking
		else if (Input.GetButtonUp("Fire1"))
		{
			if (rayActive)
			{
				CancelFrostRay();
			}
			CancelAttack();
		}
	}

	string DetermineAttackState()
	{
		// Calculate the current direction based on mouse position
		DirectionState newDirectionState = CalculateDirection();

		switch (newDirectionState)
		{
			case DirectionState.Right:
				return "playerRightAttack";
			case DirectionState.UpRight:
				return "playerUpRightAttack";
			case DirectionState.Up:
				return "playerUpAttack";
			case DirectionState.UpLeft:
				return "playerUpLeftAttack";
			case DirectionState.Left:
				return "playerLeftAttack";
			case DirectionState.DownLeft:
				return "playerDownLeftAttack";
			case DirectionState.Down:
				return "playerDownAttack";
			case DirectionState.DownRight:
				return "playerDownRightAttack";
			default: 
				return "playerDownRightAttack";
		}
	}

	void Movement()
	{
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

		// Calculate the current direction based on mouse position
		DirectionState newDirectionState = CalculateDirection();

		// Check if the direction has changed
		if (newDirectionState != currentDirectionState)
		{
			movementDirectionChanged = true;
			currentDirectionState = newDirectionState; // Update the current direction state
		}

		// Handle movement and set animator parameters
		HandleMovement(newDirectionState);
	}

	// mouse direction
	DirectionState CalculateDirection()
	{
		float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
		if (angle < 0)
		{
			angle += 360;
		}
		// Determine the direction state based on the angle
		if (angle >= 337.5 || angle < 22.5)        // right
			return DirectionState.Right;
		else if (angle >= 22.5 && angle < 67.5)    // upRight
			return DirectionState.UpRight;
		else if (angle >= 67.5 && angle < 112.5)   // up
			return DirectionState.Up;
		else if (angle >= 112.5 && angle < 157.5)  // upLeft
			return DirectionState.UpLeft;
		else if (angle >= 157.5 && angle < 202.5)  // left
			return DirectionState.Left;
		else if (angle >= 202.5 && angle < 247.5)  // downLeft
			return DirectionState.DownLeft;
		else if (angle >= 247.5 && angle < 292.5)  // down
			return DirectionState.Down;
		else if (angle >= 292.5 && angle < 337.5)  // downRight
			return DirectionState.DownRight;
		else
			// Default return (in case no conditions are met)
			return DirectionState.Right;
	}

	// animator mouse direction bools
	void HandleMovement(DirectionState directionState)
	{
		// Set animator parameters based on the current direction state
		animator.SetBool("right", directionState == DirectionState.Right);
		animator.SetBool("upRight", directionState == DirectionState.UpRight);
		animator.SetBool("up", directionState == DirectionState.Up);
		animator.SetBool("upLeft", directionState == DirectionState.UpLeft);
		animator.SetBool("left", directionState == DirectionState.Left);
		animator.SetBool("downLeft", directionState == DirectionState.DownLeft);
		animator.SetBool("down", directionState == DirectionState.Down);
		animator.SetBool("downRight", directionState == DirectionState.DownRight);
	}

	void CancelFrostRay()
	{
		Destroy(frostImpactInstance);
		Destroy(frostImpactInstance2);
		Destroy(frostImpactInstance3);
		Destroy(frostStartInstance);
		lineRenderer.enabled = false;
		lineRenderer2.enabled = false;
		lineRenderer3.enabled = false;
		rayActive = false;
		secondRayActive = false;

		audioManager.Stop("ColdCast");
		frostSoundIsPlaying = false;
	}
	void CancelAttack()
	{
		animator.SetBool("Attack", false);
		animator.SetFloat("AttackSpeed", 1.0f);
	}

	// now calls from animator
	void Shoot()
	{
		if (!isShootingCooldown)
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
			StartCoroutine(ShootingCooldown());
		}	

	}

	private IEnumerator ShootingCooldown()
	{
		isShootingCooldown = true; // Set the cooldown flag
		yield return new WaitForSeconds(shootingCooldownDuration); // Wait for the		cooldown duration
		isShootingCooldown = false; // Reset the cooldown flag
	}

	void FixedUpdate()
	{
		CheckForLoot();

		if (!knockback && !isDashing)
		{
			rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);  // movement
		}

		aim = mousePos - rb.position;
		aim2 = ShootPoint2.transform.position - RotatePoint.transform.position;
		aim3 = ShootPoint3.transform.position - RotatePoint.transform.position;
		aim4 = ShootPoint4.transform.position - RotatePoint.transform.position;
		aim5 = ShootPoint5.transform.position - RotatePoint.transform.position;

		var rotateAngle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
		RotatePoint.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
	}

	void CheckForLoot()
	{
		Collider2D[] loot = Physics2D.OverlapCircleAll(transform.position, pickUpRadius, lootLayerMask);
		foreach (Collider2D i in loot)
		{
			i.GetComponent<Loot>().PlayerInRadius();
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
		cam.GetComponent<CameraMovement>().CameraSpeed(100.0f);
		yield return new WaitForSeconds(0.3f);
		cam.GetComponent<CameraMovement>().CameraSpeed(30.0f);

		rb.velocity = Vector3.zero;
		isDashing = false;
		gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
		gameObject.GetComponent<CircleCollider2D>().enabled = false;

		yield return new WaitForSeconds(dashCooldown);
		isDashOnCooldown = false;
	}

	void ChangeWeaponTypeMouse()
	{
		if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
		{
			// next weapon
			switch (weaponSelected)
			{
				case 0: // if now cold chosen
				// select toxic
					if (rayActive)
					{
						CancelFrostRay();
						CancelAttack();
					}

					weaponType = WeaponGreenPrefab;
					projectilesAmount = projectiles[1];
					attackSpeed = attackSpeedToxic;
					bulletForce = bulletForceToxic;
					selectWeapon(1);
					animator.SetInteger("weaponType", 1);
					weaponSelected = 1;
					WeaponChangeStart();
					break;
				case 1: // if now toxic
				// select dark
					weaponType = WeaponPurplePrefab;
					projectilesAmount = projectiles[2];
					attackSpeed = attackSpeedDark;
					bulletForce = bulletForceDark;
					selectWeapon(2);
					animator.SetInteger("weaponType", 2);
					weaponSelected = 2;
					WeaponChangeStart();
					break;
				case 2: // if now dark
				// select fire
					weaponType = WeaponRedPrefab;
					projectilesAmount = projectiles[3];
					attackSpeed = attackSpeedFire;
					bulletForce = bulletForceFire;
					selectWeapon(3);
					animator.SetInteger("weaponType", 3);
					weaponSelected = 3;
					WeaponChangeStart();
					break;
				case 3: // if now fire
				// select lightning
					weaponType = WeaponYellowPrefab;
					projectilesAmount = projectiles[4];
					attackSpeed = attackSpeedLightning;
					bulletForce = bulletForceLightning;
					selectWeapon(4);
					animator.SetInteger("weaponType", 4);
					weaponSelected = 4;
					WeaponChangeStart();
					break;
				case 4: // if now lightning
				// select cold
					weaponType = WeaponBluePrefab;
					selectWeapon(0);
					projectilesAmount = projectiles[0];
					attackSpeed = attackSpeedCold;
					animator.SetInteger("weaponType", 0);
					weaponSelected = 0;
					WeaponChangeStart();
					break;
			}
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
		{
			// previous weapon
			switch (weaponSelected)
			{
				case 0: // if now cold chosen
				// select lightning
					if (rayActive)
					{
						CancelFrostRay();
						CancelAttack();
					}

					weaponType = WeaponYellowPrefab;
					projectilesAmount = projectiles[4];
					attackSpeed = attackSpeedLightning;
					bulletForce = bulletForceLightning;
					selectWeapon(4);
					animator.SetInteger("weaponType", 4);
					weaponSelected = 4;
					WeaponChangeStart();
					break;
				case 1: // if now toxic
				// select cold
					weaponType = WeaponBluePrefab;
					selectWeapon(0);
					projectilesAmount = projectiles[0];
					attackSpeed = attackSpeedCold;
					animator.SetInteger("weaponType", 0);
					weaponSelected = 0;
					WeaponChangeStart();
					break;
				case 2: // if now dark
				// select toxic
					weaponType = WeaponGreenPrefab;
					projectilesAmount = projectiles[1];
					attackSpeed = attackSpeedToxic;
					bulletForce = bulletForceToxic;
					selectWeapon(1);
					animator.SetInteger("weaponType", 1);
					weaponSelected = 1;
					WeaponChangeStart();
					break;
				case 3: // if now fire
				// select dark
					weaponType = WeaponPurplePrefab;
					projectilesAmount = projectiles[2];
					attackSpeed = attackSpeedDark;
					bulletForce = bulletForceDark;
					selectWeapon(2);
					animator.SetInteger("weaponType", 2);
					weaponSelected = 2;
					WeaponChangeStart();
					break;
				case 4: // if now lightning
				// select fire
					weaponType = WeaponRedPrefab;
					projectilesAmount = projectiles[3];
					attackSpeed = attackSpeedFire;
					bulletForce = bulletForceFire;
					selectWeapon(3);
					animator.SetInteger("weaponType", 3);
					weaponSelected = 3;
					WeaponChangeStart();
					break;
			}
		}
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
				weaponSelected = 0;
				WeaponChangeStart();
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
				weaponSelected = 1;
				if (rayActive)
				{
					CancelFrostRay();
					CancelAttack();
				}
				WeaponChangeStart();
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
				weaponSelected = 2;

				if (rayActive)
				{
					CancelFrostRay();
					CancelAttack();
				}
				WeaponChangeStart();
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
				weaponSelected = 3;
				if (rayActive)
				{
					CancelFrostRay();
					CancelAttack();
				}
				WeaponChangeStart();
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
				weaponSelected = 4;
				if (rayActive)
				{
					CancelFrostRay();
					CancelAttack();
				}
				WeaponChangeStart();
			}
		}
	}

	void WeaponChangeStart()
	{
		animator.Play("ChangeWeaponType");
		isChangingWeapon = true;
		animator.SetBool("changeWeapon", true);
		animator.SetBool("isMoving", false);
		CancelAttack();
		rb.velocity = Vector3.zero;
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	void WeaponChanged()
	{
		isChangingWeapon = false;
		animator.SetBool("changeWeapon", false);
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
	}

	public IEnumerator ShieldCoroutine()
	{
		yield return new WaitForSeconds(shieldTimer);
		shieldAmount = 0;
		shield.SetActive(false);
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
						shield.SetActive(false);
					}
					return;
				}
				else if (shieldAmount < damage)
				{
					damage -= shieldAmount;
					shieldAmount = 0;
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
				Death();
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
			Death();
		}
	}

	void Death()
	{
		Time.timeScale = 0f;
		audioManager.Stop("SagaOfTheSeaWolves");
		CanvasUI.GetComponent<LootProgress>().PlayerDeath();
		CanvasUI.GetComponent<PauseMenu>().gameOver = true;
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
		if (isFrozen == true || shieldAmount > 0)      // if coroutine
		{
			return;
		}
		StartCoroutine(FreezeCoroutine(1));           // start new
	}

	public IEnumerator FreezeCoroutine(float seconds)
	{
		Color freezeColor = new Color(1, 1, 1, 0.7f);
		if(rayActive)
		{
			CancelFrostRay();
		}
		CancelAttack();
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
		if (shieldAmount > 0)
		{
			return;
		}

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
		InvokeRepeating("TakeBurnDamage", 0.3f, 0.3f);
		yield return new WaitForSeconds(2);
		CancelInvoke("TakeBurnDamage");
		isBurning = false;
		burnAnimator.ResetTrigger("Burn");
	}

	void ShootRay()
	{
		Vector3 direction = new Vector3(mousePos.x - ShootPoint1.transform.position.x, mousePos.y - ShootPoint1.transform.position.y, 0);
		float distance = Vector2.Distance(mousePos, ShootPoint1.transform.position);
		RaycastHit2D target = Physics2D.Raycast(ShootPoint1.transform.position, direction, distance, ~mask); // ray to direction
		Vector3 impactPoint = mousePos; // default impact point to mouse

		if (target)
		{
			impactPoint = target.point; // impact point if ray collides
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

	void ShootAdditionalRays()
	{
		// ray 2
		Vector3 direction = new Vector3(mousePos.x - ShootPoint2.transform.position.x, mousePos.y - ShootPoint2.transform.position.y, 0);
		float distance = Vector2.Distance(mousePos, ShootPoint2.transform.position);
		direction = Quaternion.AngleAxis(-30, Vector3.forward) * direction;
		Vector3 impactPoint = direction + ShootPoint2.transform.position;
		// ray 3
		Vector3 direction1 = new Vector3(mousePos.x - ShootPoint3.transform.position.x, mousePos.y - ShootPoint3.transform.position.y, 0);
		float distance1 = Vector2.Distance(mousePos, ShootPoint3.transform.position);
		direction1 = Quaternion.AngleAxis(30, Vector3.forward) * direction1;
		Vector3 impactPoint1 = direction1 + ShootPoint3.transform.position;

		RaycastHit2D target = Physics2D.Raycast(ShootPoint2.transform.position, direction, distance, ~mask); // ray to direction
		RaycastHit2D target1 = Physics2D.Raycast(ShootPoint3.transform.position, direction1, distance1, ~mask); // ray to direction

		if (target)
		{
			impactPoint = target.point; // impact point if ray collides
		}
		if (target1)
		{
			impactPoint1 = target1.point; // impact point if ray collides
		}
		lineRenderer2.SetPosition(0, ShootPoint2.transform.position); // line from start point
		lineRenderer2.SetPosition(1, impactPoint);                       // line to end point

		lineRenderer3.SetPosition(0, ShootPoint3.transform.position); // line from start point
		lineRenderer3.SetPosition(1, impactPoint1);                       // line to end point

		if (!secondRayActive) // if no impact instance
		{
			frostImpactInstance2 = Instantiate(frostRayImpactPrefab, impactPoint, Quaternion.identity); // instantiate prefab to end point
			frostImpactInstance2.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

			frostImpactInstance3 = Instantiate(frostRayImpactPrefab, impactPoint1, Quaternion.identity); // instantiate prefab to end point
			frostImpactInstance3.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg);
			secondRayActive = true;
		}
		else // if already exist
		{
			// move it
			frostImpactInstance2.transform.position = impactPoint;
			frostImpactInstance2.transform.rotation = Quaternion.identity;
			frostImpactInstance2.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

			frostImpactInstance3.transform.position = impactPoint1;
			frostImpactInstance3.transform.rotation = Quaternion.identity;
			frostImpactInstance3.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg);
		}
		lineRenderer2.enabled = true;
		lineRenderer3.enabled = true;
	}

	// shoot 1 straight
	void ShootOneProjectile()
	{
		aim.Normalize();                                                                        // magnitude to 0.
		GameObject bullet = Instantiate(weaponType, ShootPoint1.transform.position, Quaternion.identity);   // instantiate bullet.
		Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();                              // Get RB.

		if (spread)
		{
			Vector2 aimWithSpread = new Vector2();
			float spread = Random.Range(-20, 20);
			aimWithSpread = Quaternion.AngleAxis(spread, Vector3.forward) * aim;

			rbBullet.transform.Rotate(0.0f, 0.0f, Mathf.Atan2(aimWithSpread.y, aimWithSpread.x) * Mathf.Rad2Deg);       // Rotate bullet.
			rbBullet.AddForce(aimWithSpread * bulletForce, ForceMode2D.Impulse);                              // Add force to bullet.
			return;
		}

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
