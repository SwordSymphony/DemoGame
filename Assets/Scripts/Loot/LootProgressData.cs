using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootProgressData
{
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

	public LootProgressData(LootProgress lootProgress)
	{
		if (lootProgress is not null)
		{
			sapphiresAmount = lootProgress.sapphiresAmount;
			emeraldsAmount = lootProgress.emeraldsAmount;
			amethystsAmount = lootProgress.amethystsAmount;
			rubiesAmount = lootProgress.rubiesAmount;
			coinsAmount = lootProgress.coinsAmount;

			fishEyesAmount = lootProgress.fishEyesAmount;
			mushroomsAmount = lootProgress.mushroomsAmount;
			wingsAmount = lootProgress.wingsAmount;
			skullsAmount = lootProgress.skullsAmount;
			hornsAmount = lootProgress.hornsAmount;
		}
	}
}
