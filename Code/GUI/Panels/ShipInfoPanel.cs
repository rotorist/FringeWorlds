using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfoPanel : PanelBase
{
	public GameObject ShipInfoSheet;
	public GameObject ShipDataSheet;

	public List<GameObject> AllSheets;

	public UILabel HullValue;
	public UILabel TurnRateValue;
	public UILabel WeaponCapValue;
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

	public UILabel ShipName;
	public UILabel ShipDescription;

	public GameObject ShipImageAnchor;
	public UISprite ShipImage;

	public UITabSelection Tabs;

	public override void Initialize ()
	{
		base.Initialize();
		//GameManager.Inst.DBManager.JsonDataHandler.LoadShipStatJson("spitfire");

	}

	public override void Show ()
	{
		base.Show();
		GameManager.Inst.CameraController.SetCameraBlur(20f, true);
		OnTabSelect("Ship");
	}

	public override void Hide ()
	{
		base.Hide();
		GameManager.Inst.CameraController.SetCameraBlur(20f, false);
	}

	public override void OnTabSelect (string tabName)
	{
		foreach(GameObject go in AllSheets)
		{
			NGUITools.SetActive(go, false);
		}

		if(tabName == "Ship")
		{
			NGUITools.SetActive(ShipInfoSheet, true);
			NGUITools.SetActive(ShipDataSheet, true);
			RefreshShipInfo();
		}
	}


	private void RefreshShipInfo()
	{
		string shipID = GameManager.Inst.PlayerProgress.ActiveLoadout.ShipID;
		ShipStats stats = GameManager.Inst.ItemManager.AllShipStats[shipID];

		ShipName.text = stats.DisplayName;
		ShipDescription.text = stats.Description;
		if(ShipImage != null)
		{
			GameObject.Destroy(ShipImage.gameObject);
		}
		ShipImage = GameManager.Inst.UIManager.LoadUISprite("ShipIcon" + shipID, ShipImageAnchor.transform, 230, 172, 10);

		HullValue.text = stats.Hull.ToString();
		TurnRateValue.text = stats.TurnRate.ToString();
		WeaponCapValue.text = stats.WeaponCapacitor + " / " + stats.WeaponCapacitorRecharge;
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
