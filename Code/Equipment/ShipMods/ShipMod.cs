using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMod
{
	public ShipModType Type;
	public Dictionary<string, float> NumericAttributes;

	public virtual void Initialize(InvItemData shipModItem)
	{
		NumericAttributes = new Dictionary<string, float>();
		foreach(ItemAttribute attribute in shipModItem.Item.Attributes)
		{
			if(attribute.Name != "Power Required" && attribute.Name != "Active Mod Class Name" && attribute.Name != "Dependency" && attribute.Name != "Equipment Type")
			{
				NumericAttributes.Add(attribute.Name, shipModItem.Item.GetFloatAttribute(attribute.Name));
			}
		}
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
