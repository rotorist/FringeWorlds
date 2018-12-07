using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModShieldCapacitor : ShipMod
{
	public ShipBase MyShip;
	public ShieldBase MyShield;

	private float _capacity;
	private float _currentCharge;

	public override void Initialize (InvItemData shipModItem)
	{
		base.Initialize (shipModItem);
	}

	public override void PerFrameUpdate ()
	{
		if(MyShield != null)
		{
			
			//use ship's shield recharge rate to remove charges from the shield and add to capacitor
			float charge = MyShield.RechargeRate * Time.deltaTime;
			if(MyShield.Amount > 0 && _currentCharge < _capacity)
			{
				MyShield.Amount = Mathf.Clamp(MyShield.Amount - charge, 0, MyShield.TotalCapacity);
				_currentCharge = Mathf.Clamp(_currentCharge + charge, 0, _capacity);
			}

			if(_currentCharge >= _capacity)
			{
				IsReady = true;
			}
		}
	}

	public override void ApplyModToShip (ShipBase ship)
	{
		MyShip = ship;
		MyShield = ship.Shield;
		_currentCharge = 0;
		_capacity = NumericAttributes["Capacity"];
	}

	public override void Deploy ()
	{
		if(MyShield != null && MyShield.Amount < MyShield.TotalCapacity && IsReady)
		{
			_currentCharge = 0;
			MyShield.Amount = Mathf.Clamp(MyShield.Amount + _capacity, 0, MyShield.TotalCapacity);
			IsReady = false;
		}
	}
}
