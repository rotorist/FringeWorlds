using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModHull : ShipMod
{


	public override void ApplyModToShip (ShipBase ship)
	{
		
		foreach(KeyValuePair<string,float> attribute in NumericAttributes)
		{
			switch(attribute.Key)
			{
			case "Capacity":
				ship.HullCapacity += attribute.Value;
				ship.HullAmount += attribute.Value;
				break;
			case "Capacity%":
				ship.HullCapacity += ship.HullCapacity * attribute.Value;
				ship.HullAmount += ship.HullAmount * attribute.Value;
				break;
			}

		}
	}
}
