using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDataSheet : PanelBase
{
	public UILabel HullValue;
	public UILabel TurnRateValue;
	public UILabel PowerSupplyValue;
	public UILabel ModSlotsValue;
	public UILabel FASpeedValue;
	public UILabel AccelerationValue;
	public UILabel CruiseSpeedValue;
	public UILabel CruiseDelayValue;
	public UILabel FuelValue;
	public UILabel LifeSupportValue;
	public UILabel StorageValue;
	public UILabel ShieldClassValue;
	public UILabel WeaponJointsValue;

	public override void Initialize ()
	{
		base.Initialize();

	}

	public override void Show ()
	{
		base.Show();
	}

	public override void Hide ()
	{
		base.Hide();
	}

	public void Refresh()
	{
		string shipID = GameManager.Inst.PlayerProgress.ActiveLoadout.ShipID;
		ShipStats stats = GameManager.Inst.ItemManager.AllShipStats[shipID];

		HullValue.text = stats.Hull.ToString();
		TurnRateValue.text = stats.TurnRate.ToString();
		PowerSupplyValue.text = stats.PowerSupply.ToString();
		ModSlotsValue.text = stats.ModSlots.ToString();
		FASpeedValue.text = stats.MaxSpeed.ToString();
		AccelerationValue.text = stats.Acceleration.ToString();
		CruiseSpeedValue.text = stats.CruiseSpeed.ToString();
		CruiseDelayValue.text = stats.CruisePrepTime.ToString();
		FuelValue.text = stats.MaxFuel.ToString();
		LifeSupportValue.text = stats.LifeSupport.ToString();
		ShieldClassValue.text = stats.ShieldClass.ToString();
		StorageValue.text = stats.AmmoBaySize + " + " + stats.CargoBaySize;
		WeaponJointsValue.text = "";
		foreach(WeaponJointData joint in stats.WeaponJoints)
		{
			string line = "Class " + joint.Class + " ";
			if(joint.RotationType == WeaponRotationType.Gimball)
			{
				line += "Gun/Missile";
			}
			else if(joint.RotationType == WeaponRotationType.Turret)
			{
				line += "Turret";
			}
			line += '\n';
			WeaponJointsValue.text += line;
		}
		for(int i=0; i<stats.DefenseSlots; i++)
		{
			WeaponJointsValue.text += "Defense Equipment" + '\n';
		}
	}
}
