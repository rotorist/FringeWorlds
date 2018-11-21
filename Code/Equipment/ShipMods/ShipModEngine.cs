using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModEngine : ShipMod
{


	public override void ApplyModToShip (ShipBase ship)
	{

		foreach(KeyValuePair<string,float> attribute in NumericAttributes)
		{
			Engine engine = ship.Engine;
			switch(attribute.Key)
			{
			case "MaxSpeed":
				engine.MaxSpeed += attribute.Value;
				break;
			case "MaxSpeed%":
				engine.MaxSpeed += engine.MaxSpeed * attribute.Value;
				break;
			case "Acceleration":
				engine.Acceleration += attribute.Value;
				break;
			case "Acceleration%":
				engine.Acceleration += engine.Acceleration * attribute.Value;
				break;
			case "CruiseSpeed":
				engine.CruiseSpeed += attribute.Value;
				break;
			case "CruiseSpeed%":
				engine.CruiseSpeed += engine.CruiseSpeed * attribute.Value;
				break;
			case "CruisePrepTime":
				engine.CruisePrepTime += attribute.Value;
				break;
			case "CruisePrepTime%":
				engine.CruisePrepTime += engine.CruisePrepTime * attribute.Value;
				break;
				//max fuel and life support will be covered in storage mods
			}

		}
	}
}

