using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HomeStationData
{
	public List<Loadout> ShipsInHangar;
	public List<InvItemData> ItemsInVault;
	public int HangarSize;
	public int VaultSize;

	public HomeStationData()
	{
		ShipsInHangar = new List<Loadout>();
		ItemsInVault = new List<InvItemData>();
	}
}

[System.Serializable]
public class HomeStationSaveData
{
	public int HangarSize;
	public int VaultSize;
	public List<LoadoutSaveData> ShipsInHangar;
	public List<InvItemData> ItemsInVault;
}