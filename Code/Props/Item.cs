using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Item
{
	public string ID;
	public string DisplayName;
	public string PrefabName;
	public string Description;
	public ItemType Type;
	public float CargoUnits;
	public float Health;//0 to 1

	public List<ItemAttribute> Attributes;
	public Dictionary<string, int> AttributeIndex;

	public Item()
	{
		Attributes = new List<ItemAttribute>();
		AttributeIndex = new Dictionary<string, int>();
	}

	public Item(ItemStats stats)
	{
		//create an item based on a stats template
		ID = stats.ID;
		DisplayName = stats.DisplayName;
		Description = stats.Description;
		PrefabName = stats.PrefabName;
		Type = stats.Type;
		CargoUnits = stats.CargoUnits;
		Health = 1;
		Attributes = new List<ItemAttribute>();
		foreach(ItemAttribute attribute in stats.Attributes)
		{
			ItemAttribute newAttribute = new ItemAttribute(attribute.Name, attribute.SerValue);
			Attributes.Add(newAttribute);
		}

		AttributeIndex = new Dictionary<string, int>();
		BuildIndex();
	}

	public Item(Item item)
	{
		//clone an existing item
		ID = item.ID;
		DisplayName = item.DisplayName;
		PrefabName = item.PrefabName;
		Description = item.Description;
		Type = item.Type;
		CargoUnits = item.CargoUnits;
		Health = item.Health;
		Attributes = new List<ItemAttribute>();
		foreach(ItemAttribute attribute in item.Attributes)
		{
			ItemAttribute newAttribute = new ItemAttribute(attribute.Name, attribute.Value);
			Attributes.Add(newAttribute);
		}

		AttributeIndex = new Dictionary<string, int>();
		BuildIndex();
	}

	public string GetStringAttribute(string name)
	{
		return GetAttributeByName(name).Value.ToString();
	}

	public float GetFloatAttribute(string name)
	{
		return Convert.ToSingle(GetAttributeByName(name).Value.ToString());
	}

	public float GetIntAttribute(string name)
	{
		return Convert.ToInt32(GetAttributeByName(name).Value.ToString());
	}

	public ItemAttribute GetAttributeByName(string name)
	{
		if(AttributeIndex.Count > 0)
		{
			if(AttributeIndex.ContainsKey(name))
			{
				return Attributes[AttributeIndex[name]];
			}
			else
			{
				return null;
			}
		}
		else
		{
			return null;
		}

	}

	public void SetAttribute(string name, object value)
	{
		if(AttributeIndex.ContainsKey(name))
		{
			Attributes[AttributeIndex[name]].Value = value;
		}
	}

	public void BuildIndex()
	{
		AttributeIndex.Clear();
		for(int i=0; i<Attributes.Count; i++)
		{
			if(Attributes[i] != null)
			{
				AttributeIndex.Add(Attributes[i].Name, i);
			}
		}
	}

	public void PostLoad()
	{
		BuildIndex();
	}
}



public enum ItemType
{
	Ammo = 1,
	Commodity = 2,
	Weapon = 3,
	Fuel = 4,
	LifeSupport = 5,
	ShipMod = 6,
	Equipment = 7,
}


[System.Serializable]
public class ItemStats
{
	public string ID;
	public string DisplayName;
	public string PrefabName;
	public string Description;
	public ItemType Type;
	public float CargoUnits;

	public List<ItemAttribute> Attributes;
}