using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrades : MonoBehaviour
{
	public GameObject player;
	// public static bool gameIsPaused = false;
	public GameObject LevelUpMenu;

	public GameObject WeaponBluePrefab;   // weapon against blue enemies
	public GameObject WeaponGreenPrefab;  // weapon against green enemies
	public GameObject WeaponPurplePrefab; // weapon against purple enemies
	public GameObject WeaponRedPrefab;    // weapon against red enemies
	public GameObject WeaponYellowPrefab; // weapon against yellow enemies

	public int maxLevelProjectiles;
	public int maxLevelAttack;

	public Upgrade Upgrade_1;
	public Upgrade Upgrade_2;
	public Upgrade Upgrade_3;

	[SerializeField] private Button Upgrade_button1;
	[SerializeField] private Button Upgrade_button2;
	[SerializeField] private Button Upgrade_button3;

	private Text Upgrade_DescriptionText1;
	private Text Upgrade_DescriptionText2;
	private Text Upgrade_DescriptionText3;

	private Text Upgrade_TypeText1;
	private Text Upgrade_TypeText2;
	private Text Upgrade_TypeText3;

	private Text Upgrade_LevelText1;
	private Text Upgrade_LevelText2;
	private Text Upgrade_levelText3;

	// lists of Upgrades of each type
	List<Upgrade> UpgradesList = new List<Upgrade>()
	{
		// new Upgrade { Name = "Attack speed", Description = "Reduces delay between attacks", Type = "Cold", Level = 0 },
		new Upgrade { Name = "Attack damage", Description = "Increases damage", Type = "Cold", Level = 0 },
	// 	new Upgrade { Name = "Attack size", Description = "Increases size of attack", Type = "Cold", Level = 0 },
	// 	new Upgrade { Name = "Attack pierce", Description = "Increase number of enemies that projectile can pierce", Type = "Cold", Level = 0 },
		new Upgrade { Name = "Attack AoE Increase", Description = "AoE increase", Type = "Cold", Level = 0 },
		new Upgrade { Name = "Rays number", Description = "increases number of rays", Type = "Cold", Level = 0 },

		new Upgrade { Name = "Attack speed", Description = "Reduces delay between attacks", Type = "Toxic", Level = 0 },
		new Upgrade { Name = "Attack damage", Description = "Increases damage", Type = "Toxic", Level = 0 },
	// 	new Upgrade { Name = "Attack size", Description = "Increases size of attack", Type = "Toxic", Level = 0 },
	// 	new Upgrade { Name = "Attack pierce", Description = "Increase number of enemies that projectile can pierce", Type = "Toxic", Level = 0 },
		new Upgrade { Name = "Attack AoE Increase", Description = "AoE increase", Type = "Toxic", Level = 0 },
		new Upgrade { Name = "Projectiles number", Description = "increases number of projectiles", Type = "Toxic", Level = 0 },

		new Upgrade { Name = "Attack speed", Description = "Reduces delay between attacks", Type = "Dark", Level = 0 },
		new Upgrade { Name = "Attack damage", Description = "Increases damage", Type = "Dark", Level = 0 },
	// 	new Upgrade { Name = "Attack size", Description = "Increases size of attack", Type = "Dark", Level = 0 },
	// 	new Upgrade { Name = "Attack pierce", Description = "Increase number of enemies that projectile can pierce", Type = "Dark", Level = 0 },
		new Upgrade { Name = "Attack AoE Increase", Description = "AoE increase", Type = "Dark", Level = 0 },
		new Upgrade { Name = "Projectiles number", Description = "increases number of projectiles", Type = "Dark", Level = 0 },

		new Upgrade { Name = "Attack speed", Description = "Reduces delay between attacks", Type = "Fire", Level = 0 },
		new Upgrade { Name = "Attack damage", Description = "Increases damage", Type = "Fire", Level = 0 },
	// 	new Upgrade { Name = "Attack size", Description = "Increases size of attack", Type = "Fire", Level = 0 },
	// 	new Upgrade { Name = "Attack pierce", Description = "Increase number of enemies that projectile can pierce", Type = "Fire", Level = 0 },
		new Upgrade { Name = "Attack AoE Increase", Description = "AoE increase", Type = "Fire", Level = 0 },
		new Upgrade { Name = "Projectiles number", Description = "increases number of projectiles", Type = "Fire", Level = 0 },

		new Upgrade { Name = "Attack speed", Description = "Reduces delay between attacks", Type = "Lightning", Level = 0 },
		new Upgrade { Name = "Attack damage", Description = "Increases damage", Type = "Lightning", Level = 0 },
	// 	new Upgrade { Name = "Attack size", Description = "Increases size of attack", Type = "Lightning", Level = 0 },
	// 	new Upgrade { Name = "Attack pierce", Description = "Increase number of enemies that projectile can pierce", Type = "Lightning", Level = 0 },
		new Upgrade { Name = "Attack AoE Increase", Description = "AoE increase", Type = "Lightning", Level = 0 },
		new Upgrade { Name = "Projectiles number", Description = "increases number of projectiles", Type = "Lightning", Level = 0 }
	};

	void Start()
	{
		maxLevelProjectiles = 4;
		maxLevelAttack = 3;
		GetRandomUpgradesList();
	}

	public void GetRandomUpgradesList()
	{
		int upgrade1 = Random.Range(0, UpgradesList.Count);
		int upgrade2 = Random.Range(0, UpgradesList.Count);
		int upgrade3 = Random.Range(0, UpgradesList.Count);

		Upgrade_1 = UpgradesList[upgrade1];
		Upgrade_2 = UpgradesList[upgrade2];
		Upgrade_3 = UpgradesList[upgrade3];

		Upgrade_button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Upgrade_1.Name;
		Upgrade_button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Upgrade_2.Name;
		Upgrade_button3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Upgrade_3.Name;

		Upgrade_button1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Upgrade_1.Type;
		Upgrade_button2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Upgrade_2.Type;
		Upgrade_button3.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Upgrade_3.Type;

		Upgrade_button1.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Upgrade_1.Description;
		Upgrade_button2.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Upgrade_2.Description;
		Upgrade_button3.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Upgrade_3.Description;

		// Setting color of the buttons
		Dictionary<string, Color> TypeColors = new Dictionary<string, Color>();
		TypeColors.Add("Cold", new Color(0f, 1f, 1f, 1f));
		TypeColors.Add("Toxic", new Color(0f, 1f, 0f, 1f));
		TypeColors.Add("Dark", new Color(0.584f, 0.180f, 0.560f));
		TypeColors.Add("Fire", new Color(1f, 0f, 0f, 1f));
		TypeColors.Add("Lightning", new Color(1f, 0.92f, 0.016f, 1f));

		Upgrade_button1.GetComponent<Image>().color = TypeColors[Upgrade_1.Type];
		Upgrade_button2.GetComponent<Image>().color = TypeColors[Upgrade_2.Type];
		Upgrade_button3.GetComponent<Image>().color = TypeColors[Upgrade_3.Type];
	}

	public void SelectUpgrade(int number)
	{
		if (number == 1)
		{
			UpgradeChosen(Upgrade_1);
		}
		else if (number == 2)
		{
			UpgradeChosen(Upgrade_2);
		}
		else if (number == 3)
		{
			UpgradeChosen(Upgrade_3);
		}
	}

	public void UpgradeChosen(Upgrade _upgrade)
	{
		string type = _upgrade.Type;
		string name = _upgrade.Name;
		var _player = player.GetComponent<Player>();

		if (type == "Cold")
		{
			Debug.Log("todo: rework blue prefab and ray");
			switch (name)
			{
				case "Rays number":
					// _player.projectiles[0] += 1; todo: add ray
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelProjectiles)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				// case "Attack speed":
				// 	_player.cooldownCold -= 0.03f;
				// 	_upgrade.Level += 1;
				// 	if (_upgrade.Level == maxLevelAttack)
				// 	{
				// 		UpgradesList.Remove(_upgrade);
				// 	}
				// 	break;

				case "Attack damage":
					_player.damageCold += 2;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack AoE Increase":
					_player.coldAoeSize *= 1.3f;
					_player.coldImpactSize *= 1.3f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case null:
					break;
			}
		}
		else if (type == "Toxic")
		{
			switch (name)
			{
				case "Projectiles number":
					_player.projectiles[1] += 1;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelProjectiles)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack speed":
					_player.cooldownToxic -= 0.11f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack damage":
					_player.damageToxic += 10;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack AoE Increase":
					_player.toxicAoeSize *= 1.3f;
					_player.toxicImpactSize *= 1.3f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case null:
					break;
			}
		}
		else if (type == "Dark")
		{
			switch (name)
			{
				case "Projectiles number":
					_player.projectiles[2] += 1;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelProjectiles)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack speed":
					_player.cooldownDark -= 0.11f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack damage":
					_player.damageDark += 10;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack AoE Increase":
					_player.darkAoeSize *= 1.3f;
					_player.darkImpactSize *= 1.3f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case null:
					break;
			}
		}
		else if (type == "Fire")
		{
			switch (name)
			{
				case "Projectiles number":
					_player.projectiles[3] += 1;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelProjectiles)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack speed":
					_player.cooldownFire -= 0.11f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack damage":
					_player.damageFire += 10;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack AoE Increase":
					_player.fireAoeSize *= 1.3f;
					_player.fireImpactSize *= 1.3f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case null:
					break;
			}
		}
		else if (type == "Lightning")
		{
			switch (name)
			{
				case "Projectiles number":
					_player.projectiles[4] += 1;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelProjectiles)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack speed":
					_player.cooldownLightning -= 0.11f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack damage":
					_player.damageLightning += 10;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case "Attack AoE Increase":
					_player.lightningAoeSize *= 1.3f;
					_player.lightningImpactSize *= 1.3f;
					_upgrade.Level += 1;
					if (_upgrade.Level == maxLevelAttack)
					{
						UpgradesList.Remove(_upgrade);
					}
					break;

				case null:
					break;
			}
		}
		LevelUpMenu.SetActive(false);
		Time.timeScale = 1f;
		GetRandomUpgradesList();

	}

	public class Upgrade
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public int Level { get; set; }
	}
}
