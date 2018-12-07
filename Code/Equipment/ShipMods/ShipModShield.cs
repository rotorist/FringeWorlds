using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModShield : ShipMod
{


	public override void ApplyModToShip (ShipBase ship)
	{
		ShieldBase shield = ship.Shield;
		if(shield == null)
		{
			return;
		}

		foreach(KeyValuePair<string,float> attribute in NumericAttributes)
		{
			switch(attribute.Key)
			{
			case "Capacity":
				shield.TotalCapacity += attribute.Value;
				break;
			case "Capacity%":
				shield.TotalCapacity += shield.TotalCapacity * attribute.Value;
				break;
			case "RechargeRate":
				shield.RechargeRate += attribute.Value;
				break;
			case "RechargeRate%":
				shield.RechargeRate += shield.RechargeRate * attribute.Value;
				break;
			}

		}

		Debug.Log("Shield capacity " + shield.TotalCapacity);
	}
}
