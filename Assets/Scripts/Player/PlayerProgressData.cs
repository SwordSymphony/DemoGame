using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProgressData
{
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

	public float cooldownCold;
	public float cooldownToxic;
	public float cooldownDark;
	public float cooldownFire;
	public float cooldownLightning;

	public List<int> projectiles;

	public PlayerProgressData (Player player)
	{
		if (player is not null)
		{
			// damage
			damageCold = player.damageCold;
			damageToxic = player.damageToxic;
			damageDark = player.damageDark;
			damageFire = player.damageFire;
			damageLightning = player.damageLightning;

			// overlap circle of aoe
			coldAoeSize = player.coldAoeSize;
			toxicAoeSize = player.toxicAoeSize;
			darkAoeSize = player.darkAoeSize;
			fireAoeSize = player.fireAoeSize;
			lightningAoeSize = player.lightningAoeSize;

			// sprite size
			coldImpactSize = player.coldImpactSize;
			toxicImpactSize = player.toxicImpactSize;
			darkImpactSize = player.darkImpactSize;
			fireImpactSize = player.fireImpactSize;
			lightningImpactSize = player.lightningImpactSize;

			// cooldown
			cooldownCold = player.cooldownCold;
			cooldownToxic = player.cooldownToxic;
			cooldownDark = player.cooldownDark;
			cooldownFire = player.cooldownFire;
			cooldownLightning = player.cooldownLightning;

			projectiles = player.projectiles;
		}
	}
}