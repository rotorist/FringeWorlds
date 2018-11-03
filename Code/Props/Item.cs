using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
	public string ID;
	public string DisplayName;
	public string Description;
	public ItemType Type;
	public float CargoUnits;

	public List<ItemAttribute> Attributes;
	public Dictionary<string, int> AttributeIndex;

	public Item()
	{
		Attributes = new List<ItemAttribute>();
		AttributeIndex = new Dictionary<string, int>();
	}

	public Item(Item item)
	{
		//clone an existing item
		ID = item.ID;
		DisplayName = item.DisplayName;
		Description = item.Description;
		Type = item.Type;
		Attributes = new List<ItemAttribute>();
		foreach(ItemAttribute attribute in item.Attributes)
		{
			ItemAttribute newAttribute = new ItemAttribute(attribute.Name, attribute.Value);
			Attributes.Add(newAttribute);
		}

		AttributeIndex = new Dictionary<string, int>();
		BuildIndex();
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
	Ammo,
	Commodity,
	Weapon,
	Supply,
	Upgrade,

}