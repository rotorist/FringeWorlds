﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfoSheet : PanelBase
{
	public Loadout CurrentLoadout;
	public UILabel ShipName;
	public UILabel ShipDescription;

	public GameObject ShipImageAnchor;
	public UISprite ShipImage;

	public BarIndicator HullIndicator;
	public BarIndicator FuelIndicator;
	public BarIndicator LifeSupportIndicator;

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
		string shipID = CurrentLoadout.ShipID;
		ShipStats stats = GameManager.Inst.ItemManager.GetShipStats(shipID);

		ShipName.text = stats.DisplayName;
		ShipDescription.text = stats.Description;
		if(ShipImage != null)
		{
			GameObject.Destroy(ShipImage.gameObject);
		}
		ShipImage = GameManager.Inst.UIManager.LoadUISprite("ShipIcon" + shipID, ShipImageAnchor.transform, 230, 172, 10);



		float hullAmountPercent = 0;
		float fuelAmountPercent = 0;
		float lsAmountPercent = 0;
		if(GameManager.Inst.SceneType == SceneType.Space)
		{
			hullAmountPercent = GameManager.Inst.PlayerControl.PlayerShip.HullAmount / GameManager.Inst.PlayerControl.PlayerShip.HullCapacity;
			fuelAmountPercent = GameManager.Inst.PlayerControl.PlayerShip.FuelAmount / GameManager.Inst.PlayerControl.PlayerShip.MaxFuel;
			lsAmountPercent = GameManager.Inst.PlayerControl.PlayerShip.LifeSupportAmount / GameManager.Inst.PlayerControl.PlayerShip.MaxLifeSupport;
		}
		else if(GameManager.Inst.SceneType == SceneType.Station)
		{
			hullAmountPercent = CurrentLoadout.HullAmount / GameManager.Inst.ItemManager.GetShipStats(CurrentLoadout.ShipID).Hull;
			fuelAmountPercent = CurrentLoadout.FuelAmount / GameManager.Inst.ItemManager.GetShipStats(CurrentLoadout.ShipID).MaxFuel;
			lsAmountPercent = CurrentLoadout.LifeSupportAmount / GameManager.Inst.ItemManager.GetShipStats(CurrentLoadout.ShipID).LifeSupport;
		}

		HullIndicator.SetFillPercentage(hullAmountPercent);
		FuelIndicator.SetFillPercentage(fuelAmountPercent);
		LifeSupportIndicator.SetFillPercentage(lsAmountPercent);
	}
}
