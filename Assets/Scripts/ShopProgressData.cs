using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopProgressData
{
	public bool upgradeBought1;
	public bool upgradeBought2;
	public bool upgradeBought3;
	public bool upgradeBought4;
	public bool upgradeBought5;

	public ShopProgressData (ShopMenu shopMenu)
	{
		if (shopMenu is not null)
		{
			upgradeBought1 = shopMenu.upgradeBought1;
			upgradeBought1 = shopMenu.upgradeBought2;
			upgradeBought2 = shopMenu.upgradeBought3;
			upgradeBought3 = shopMenu.upgradeBought4;
			upgradeBought4 = shopMenu.upgradeBought5;
		}
	}
}
