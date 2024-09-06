using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameProgress : MonoBehaviour
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

	int sapphiresAmount;
	int emeraldsAmount;
	int amethystsAmount;
	int rubiesAmount;
	int coinsAmount;

	int fishEyesAmount;
	int mushroomsAmount;
	int wingsAmount;
	int skullsAmount;
	int hornsAmount;

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
			expToNextLevel += 20 + expProgression;
			expProgression += 5;
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
				LootMenu.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();
				break;
			case 2:
				mushroomsAmount += 1;
				LootMenu.transform.GetChild(6).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();
				break;
			case 3:
				wingsAmount += 1;
				LootMenu.transform.GetChild(7).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();
				break;
			case 4:
				skullsAmount += 1;
				LootMenu.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();
				break;
			case 5:
				hornsAmount += 1;
				LootMenu.transform.GetChild(9).gameObject.GetComponent<TextMeshProUGUI>().text = coinsAmount.ToString();
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
