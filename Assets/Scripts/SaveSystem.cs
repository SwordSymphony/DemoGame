using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
	public static void SaveLootProgress(LootProgress lootProgress)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/LootProgress.frog";

		FileStream stream = new FileStream(path, FileMode.Create);
		LootProgressData data = new LootProgressData(lootProgress);

		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static LootProgressData LoadLootProgress()
	{
		string path = Application.persistentDataPath + "/LootProgress.frog";

		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			LootProgressData data = formatter.Deserialize(stream) as LootProgressData;
			stream.Close();

			return data;
		}
		else
		{
			Debug.LogError("Save file not found in " + path);

			return null;
		}
	}

	public static void SavePlayerProgress(Player player)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/PlayerProgress.frog";

		FileStream stream = new FileStream(path, FileMode.Create);
		PlayerProgressData data = new PlayerProgressData(player);

		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static PlayerProgressData LoadPlayerProgress()
	{
		string path = Application.persistentDataPath + "/PlayerProgress.frog";

		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			PlayerProgressData data = formatter.Deserialize(stream) as PlayerProgressData;
			stream.Close();

			return data;
		}
		else
		{
			Debug.LogError("Save file not found in " + path);

			return null;
		}
	}

	public static void SaveShopProgress(ShopMenu shopMenu)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/ShopProgress.frog";

		FileStream stream = new FileStream(path, FileMode.Create);
		ShopProgressData data = new ShopProgressData(shopMenu);

		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static ShopProgressData LoadShopProgress()
	{
		string path = Application.persistentDataPath + "/ShopProgress.frog";

		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			ShopProgressData data = formatter.Deserialize(stream) as ShopProgressData;
			stream.Close();

			return data;
		}
		else
		{
			Debug.LogError("Save file not found in " + path);

			return null;
		}
	}
}
