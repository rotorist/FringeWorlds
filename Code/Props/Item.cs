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
	public float BasePrice;
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
		BasePrice = stats.BasePrice;
		Attributes = new List<ItemAttribute>();
		foreach(ItemAttribute attribute in stats.Attributes)
		{
			ItemAttribute newAttribute = new ItemAttribute(attribute.Name, attribute.SerValue, attribute.IsHidden, attribute.Unit);
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
		BasePrice = item.BasePrice;
		Attributes = new List<ItemAttribute>();
		foreach(ItemAttribute attribute in item.Attributes)
		{
			ItemAttribute newAttribute = new ItemAttribute(attribute.Name, attribute.Value, attribute.IsHidden, attribute.Unit);
			Attributes.Add(newAttribute);
		}

		AttributeIndex = new Dictionary<string, int>();
		BuildIndex();
	}

	public string GetStringAttribute(string name)
	{
		ItemAttribute attribute = GetAttributeByName(name);
		if(attribute != null)
		{
			return attribute.Value.ToString();
		}
		else
		{
			return "";
		}
	}

	public float GetFloatAttribute(string name)
	{
		ItemAttribute attribute = GetAttributeByName(name);
		if(attribute != null)
		{
			return Convert.ToSingle(attribute.Value.ToString());
		}
		else
		{
			return 0;
		}

	}

	public int GetIntAttribute(string name)
	{
		ItemAttribute attribute = GetAttributeByName(name);
		if(attribute != null)
		{
			return Convert.ToInt32(attribute.Value.ToString());
		}
		else
		{
			return 0;
		}
	}

	public bool GetBoolAttribute(string name)
	{
		ItemAttribute attribute = GetAttributeByName(name);
		if(attribute != null)
		{
			return Convert.ToBoolean(attribute.Value.ToString());
		}
		else
		{
			return false;
		}
	}

	public int [] GetIntArrayAttribute(string name)
	{
		string value = GetStringAttribute(name);
		string [] stringArray = value.Split(',');
		int [] intArray = new int[stringArray.Length];
		for(int i=0; i<stringArray.Length; i++)
		{
			intArray[i] = Convert.ToInt32(stringArray[i]);
		}

		return intArray;
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
	Defensives = 8,
	HangarItem = 9,
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
	public float BasePrice;
	public List<ItemAttribute> Attributes;
}