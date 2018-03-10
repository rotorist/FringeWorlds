using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBase : EquipmentBase
{
	public ShieldType Type;
	public ShieldTech Tech;
	public ShipBase ParentShip;
	public float TotalCapacity;

	public virtual Damage ProcessDamage(Damage damage)
	{

		return damage;
	}

	public virtual float GetShieldPercentage()
	{
		return 1;
	}
}

public enum ShieldType
{
	Fighter,
	Transport,
	Capitol,
}

public enum ShieldTech
{
	Magnetic,
	Gravity,
	Plasma,
}