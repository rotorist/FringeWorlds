using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : EquipmentBase 
{
	public float MaxSpeed;
	public float Acceleration;
	public float MaxFuel;
	public float CurrentFuel;
	public float ConsumptionRate;
	public float RestoreRate;
	public bool CanStrafe;

	public void Initialize(InvItemData itemData)
	{
		if(itemData == null)
		{
			MaxFuel = 0;
			CurrentFuel = 0;
		}
		else
		{
			MaxSpeed = itemData.Item.GetFloatAttribute("Top Speed");
			Acceleration = itemData.Item.GetFloatAttribute("Acceleration");
			MaxFuel = itemData.Item.GetFloatAttribute("Max Fuel");
			CurrentFuel = MaxFuel;
			ConsumptionRate = itemData.Item.GetFloatAttribute("Drain Rate");
			RestoreRate = itemData.Item.GetFloatAttribute("Restore Rate");
			CanStrafe = itemData.Item.GetBoolAttribute("Can Strafe");
			this.PowerRequired = itemData.Item.GetFloatAttribute("Power Required");
		}
	}

}
