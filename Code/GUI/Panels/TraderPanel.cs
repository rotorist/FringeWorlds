using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderPanel : PanelBase
{
	public Loadout CurrentLoadout;
	public ShipInventorySheet ShipInventorySheet;
	public TraderInventorySheet TraderInventorySheet;
	public EquipmentActionSheet EquipmentActionSheet;

	public List<GameObject> AllSheets;

	public override void Initialize ()
	{
		base.Initialize();
		//GameManager.Inst.DBManager.JsonDataHandler.LoadAllItemStats();
		ShipInventorySheet.Initialize();
		TraderInventorySheet.Initialize();
		EquipmentActionSheet.Initialize();
	}

	public override void Show ()
	{
		base.Show();
		ResetLoadout();
		ShipInventorySheet.Show();
		TraderInventorySheet.Show();
		EquipmentActionSheet.Show();


		GameManager.Inst.CameraController.SetCameraBlur(20f, true);

	}

	public override void Hide ()
	{
		base.Hide();
		GameManager.Inst.CameraController.SetCameraBlur(20f, false);

	}



	public void ResetLoadout()
	{
		CurrentLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
		TraderInventorySheet.CurrentLoadout = CurrentLoadout;
		ShipInventorySheet.CurrentLoadout = CurrentLoadout;
	}

}
