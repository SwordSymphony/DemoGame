using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootProgress : MonoBehaviour
{
	public static AudioManager audioManager;
	public GameObject sapphiresPrefab;
	public GameObject emeraldsPrefab;
	public GameObject amethystsPrefab;
	public GameObject rubiesPrefab;
	public GameObject coinsPrefab;

	public GameObject fishEyesPrefab;
	public GameObject mushroomsPrefab;
	public GameObject wingsPrefab;
	public GameObject skullsPrefab;
	public GameObject hornsPrefab;

	public GameObject Exp_orbPrefab;
	public GameObject LevelUpMenu;
	public GameObject DeathMenu;
	public GameObject LootMenu;

	public int sapphiresAmount;
	public int emeraldsAmount;
	public int amethystsAmount;
	public int rubiesAmount;
	public int coinsAmount;

	public int fishEyesAmount;
	public int mushroomsAmount;
	public int wingsAmount;
	public int skullsAmount;
	public int hornsAmount;

	public int playerLevel;
	public int currentExp;
	public int totalExp;
	public int expToNextLevel;
	public int expProgression;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		// player = GameObject.FindWithTag("Player");
		playerLevel = 1;
		expToNextLevel = 5;
		expProgression = 5;

		// load progress
		LoadProgress();
	}

	void LoadProgress()
	{
		LootProgressData data = SaveSystem.LoadLootProgress();

		if (data is not null)
		{
			sapphiresAmount = data.sapphiresAmount;
			emeraldsAmount = data.emeraldsAmount;
			amethystsAmount = data.amethystsAmount;
			rubiesAmount = data.rubiesAmount;
			coinsAmount = data.coinsAmount;

			LootMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sapphiresAmount.ToString();
			LootMenu.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = emeraldsAmount.ToString();
			LootMenu.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = amethystsAmount.ToString();
			LootMenu.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = rubiesAmount.ToString();
			LootMenu.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();

			fishEyesAmount = data.fishEyesAmount;
			mushroomsAmount = data.mushroomsAmount;
			wingsAmount = data.wingsAmount;
			skullsAmount = data.skullsAmount;
			hornsAmount = data.hornsAmount;

			LootMenu.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().text = fishEyesAmount.ToString();
			LootMenu.transform.GetChild(6).gameObject.GetComponent<TextMeshProUGUI>().text = mushroomsAmount.ToString();
			LootMenu.transform.GetChild(7).gameObject.GetComponent<TextMeshProUGUI>().text = wingsAmount.ToString();
			LootMenu.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>().text = skullsAmount.ToString();
			LootMenu.transform.GetChild(9).gameObject.GetComponent<TextMeshProUGUI>().text = hornsAmount.ToString();
		}
	}

	public void SpendGems(int gem, int price)
	{
		switch (gem)
			{
				case 0:
					sapphiresAmount -= price;
					LootMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sapphiresAmount.ToString();
					break;
				case 1:
					emeraldsAmount -= price;
					LootMenu.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = emeraldsAmount.ToString();
					break;
				case 2:
					amethystsAmount -= price;
					LootMenu.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = amethystsAmount.ToString();
					break;
				case 3:
					rubiesAmount -= price;
					LootMenu.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = rubiesAmount.ToString();
					break;
				case 4:
					coinsAmount -= price;
					LootMenu.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();
					break;
			}
	}

	public void PlayerDeath()
	{
		int totalGems = sapphiresAmount + emeraldsAmount + amethystsAmount + rubiesAmount + coinsAmount;
		DeathMenu.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = totalGems.ToString();
		DeathMenu.transform.GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sapphiresAmount.ToString();
		DeathMenu.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = emeraldsAmount.ToString();
		DeathMenu.transform.GetChild(5).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = amethystsAmount.ToString();
		DeathMenu.transform.GetChild(6).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = rubiesAmount.ToString();
		DeathMenu.transform.GetChild(7).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();

		DeathMenu.transform.GetChild(8).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = playerLevel.ToString();
		DeathMenu.SetActive(true);

		// save progress
		SaveSystem.SaveLootProgress(this);
	}

	public void AddExp()
	{
		currentExp++;
		totalExp++;
		if (currentExp >= expToNextLevel)
		{
			playerLevel++;
			Time.timeScale = 0f;
			LevelUpMenu.SetActive(true);
			currentExp = 0;
			expProgression += 5;
			expToNextLevel += expProgression;// 5 15 30 50 75 105 140 180 225 275 330 390 455 525 600 680 765 855
		}
	}

	public void AddLoot(bool rare, int type)
	{
		if (rare)
		{
			AddRareLoot(type);
		}
		else 
		{
			AddGem(type);
		}
	}

	public void AddRareLoot(int type)
	{
		switch (type)
		{
			case 1:
				fishEyesAmount += 1;
				LootMenu.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().text = fishEyesAmount.ToString();
				break;
			case 2:
				mushroomsAmount += 1;
				LootMenu.transform.GetChild(6).gameObject.GetComponent<TextMeshProUGUI>().text = mushroomsAmount.ToString();
				break;
			case 3:
				wingsAmount += 1;
				LootMenu.transform.GetChild(7).gameObject.GetComponent<TextMeshProUGUI>().text = wingsAmount.ToString();
				break;
			case 4:
				skullsAmount += 1;
				LootMenu.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>().text = skullsAmount.ToString();
				break;
			case 5:
				hornsAmount += 1;
				LootMenu.transform.GetChild(9).gameObject.GetComponent<TextMeshProUGUI>().text = hornsAmount.ToString();
				break;
		}
	}

	public void AddGem(int type)
	{
		switch (type)
		{
			case 1:
				sapphiresAmount += 1;
				LootMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sapphiresAmount.ToString();
				audioManager.PlayOneShot("PickUpLoot1");
				break;
			case 2:
				emeraldsAmount += 1;
				LootMenu.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = emeraldsAmount.ToString();
				audioManager.PlayOneShot("PickUpLoot1");
				break;
			case 3:
				amethystsAmount += 1;
				LootMenu.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = amethystsAmount.ToString();
				audioManager.PlayOneShot("PickUpLoot");
				break;
			case 4:
				rubiesAmount += 1;
				LootMenu.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = rubiesAmount.ToString();
				audioManager.PlayOneShot("PickUpLoot");
				break;
			case 5:
				coinsAmount += 1;
				LootMenu.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();
				audioManager.PlayOneShot("LootCoins");
				break;
		}
		// audioManager.PlayOneShot("PickUpLoot");
	}
}
