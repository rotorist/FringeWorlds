using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipMod
{
	public string DependencyID;
	public ShipModType Type;
	public Dictionary<string, float> NumericAttributes;
	public bool IsReady;

	public virtual void Initialize(InvItemData shipModItem)
	{
		NumericAttributes = new Dictionary<string, float>();
		foreach(ItemAttribute attribute in shipModItem.Item.Attributes)
		{
			if(attribute.Name != "Ship Mod Type" && attribute.Name != "Power Required" && attribute.Name != "Active Mod Class Name" && attribute.Name != "Dependency" && attribute.Name != "Equipment Type" && attribute.IsHidden)
			{
				NumericAttributes.Add(attribute.Name, shipModItem.Item.GetFloatAttribute(attribute.Name));
			}
		}


		DependencyID = shipModItem.Item.GetStringAttribute("Dependency");
	}

	public virtual void ApplyModToShip(ShipBase ship)
	{

	}

	public virtual void PerFrameUpdate()
	{

	}

	public virtual void Deploy()
	{

	}

}
