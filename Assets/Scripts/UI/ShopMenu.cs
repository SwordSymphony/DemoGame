using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
	public LootProgress lootProgress;
	public Player player;

	public GameObject upgradeButton1;
	public GameObject upgradeDeactivated1;

	public GameObject upgradeButton2;
	public GameObject upgradeDeactivated2;

	public GameObject upgradeButton3;
	public GameObject upgradeDeactivated3;

	public GameObject upgradeButton4;
	public GameObject upgradeDeactivated4;

	public GameObject upgradeButton5;
	public GameObject upgradeDeactivated5;

	public bool upgradeBought1;
	public bool upgradeBought2;
	public bool upgradeBought3;
	public bool upgradeBought4;
	public bool upgradeBought5;

	public bool upgradeBought1c;
	public bool upgradeBought2t;
	public bool upgradeBought3d;
	public bool upgradeBought4f;
	public bool upgradeBought5l;

	public void Awake()
	{
		LoadProgress();

		if (upgradeBought1)
		{
			upgradeButton1.SetActive(false);
			upgradeDeactivated1.SetActive(true);
		}
		if (upgradeBought2)
		{
			upgradeButton2.SetActive(false);
			upgradeDeactivated2.SetActive(true);
		}
		if (upgradeBought3)
		{
			upgradeButton3.SetActive(false);
			upgradeDeactivated3.SetActive(true);
		}
		if (upgradeBought4)
		{
			upgradeButton4.SetActive(false);
			upgradeDeactivated4.SetActive(true);
		}
		if (upgradeBought5)
		{
			upgradeButton5.SetActive(false);
			upgradeDeactivated5.SetActive(true);
		}
	}

	void LoadProgress()
	{
		ShopProgressData data = SaveSystem.LoadShopProgress();
		if (data is not null)
		{
			upgradeBought1 = data.upgradeBought1;
		}
	}

	// cold projectiles
	public void BuyUpgrade1()
	{
		if (!upgradeBought1)
		{
			int price = 3;
			if (lootProgress.sapphiresAmount >= price)
			{
				upgradeBought1 = true;
				lootProgress.SpendGems(0, price); // type, price
				player.projectiles[0] += 1;

				SaveSystem.SaveShopProgress(this);
				SaveSystem.SavePlayerProgress(player);
				SaveSystem.SaveLootProgress(lootProgress);

				upgradeButton1.SetActive(false);
				upgradeDeactivated1.SetActive(true);

				Debug.Log("upgrade bought");
			}
			else
			{
				Debug.Log("not enough gems");
			}
		}
	}

	// toxic projectiles
	public void BuyUpgrade2()
	{
		if (!upgradeBought2)
		{
			int price = 3;
			if (lootProgress.emeraldsAmount >= price)
			{
				upgradeBought2 = true;
				lootProgress.SpendGems(1, price); // type, price
				player.projectiles[1] += 1;

				SaveSystem.SaveShopProgress(this);
				SaveSystem.SavePlayerProgress(player);
				SaveSystem.SaveLootProgress(lootProgress);

				upgradeButton2.SetActive(false);
				upgradeDeactivated2.SetActive(true);

				Debug.Log("upgrade bought");
			}
			else
			{
				Debug.Log("not enough gems");
			}
		}
	}

	// dark projectiles
	public void BuyUpgrade3()
	{
		if (!upgradeBought3)
		{
			int price = 3;
			if (lootProgress.amethystsAmount >= price)
			{
				upgradeBought3 = true;
				lootProgress.SpendGems(2, price);
				player.projectiles[2] += 1;

				SaveSystem.SaveShopProgress(this);
				SaveSystem.SavePlayerProgress(player);
				SaveSystem.SaveLootProgress(lootProgress);

				upgradeButton3.SetActive(false);
				upgradeDeactivated3.SetActive(true);

				Debug.Log("upgrade bought");
			}
			else
			{
				Debug.Log("not enough gems");
			}
		}
	}

	// fire projectiles
	public void BuyUpgrade4()
	{
		if (!upgradeBought4)
		{
			int price = 3;
			if (lootProgress.rubiesAmount >= price)
			{
				upgradeBought4 = true;
				lootProgress.SpendGems(3, price); // type, price
				player.projectiles[3] += 1;

				SaveSystem.SaveShopProgress(this);
				SaveSystem.SavePlayerProgress(player);
				SaveSystem.SaveLootProgress(lootProgress);

				upgradeButton4.SetActive(false);
				upgradeDeactivated4.SetActive(true);

				Debug.Log("upgrade bought");
			}
			else
			{
				Debug.Log("not enough gems");
			}
		}
	}

	// lightning projectiles
	public void BuyUpgrade5()
	{
		if (!upgradeBought5)
		{
			int price = 3;
			if (lootProgress.coinsAmount >= price)
			{
				upgradeBought5 = true;
				lootProgress.SpendGems(4, price); // type, price
				player.projectiles[4] += 1;

				SaveSystem.SaveShopProgress(this);
				SaveSystem.SavePlayerProgress(player);
				SaveSystem.SaveLootProgress(lootProgress);

				upgradeButton5.SetActive(false);
				upgradeDeactivated5.SetActive(true);

				Debug.Log("upgrade bought");
			}
			else
			{
				Debug.Log("not enough gems");
			}
		}
	}

}
