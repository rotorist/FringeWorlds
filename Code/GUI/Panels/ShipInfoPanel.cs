using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfoPanel : PanelBase
{
	public Loadout CurrentLoadout;
	public ShipHangarSheet ShipHangarSheet;
	public ShipInfoSheet ShipInfoSheet;
	public ShipDataSheet ShipDataSheet;
	public EquipmentSheet EquipmentSheet;
	public WeaponSheet WeaponSheet;
	public ShipInventorySheet EquipmentInventorySheet;
	public EquipmentActionSheet EquipmentActionSheet;
	public VaultStorageSheet VaultStorageSheet;

	public List<GameObject> AllSheets;

	public float SidePaneXTwoPane;
	public float SidePaneXThreePane;
	public float SidePaneAngleTwoPane;
	public float SidePaneAngleThreePane;

	public GameObject LeftPane;
	public GameObject MiddlePane;
	public GameObject RightPane;

	public UITabSelection Tabs;

	public override void Initialize ()
	{
		base.Initialize();
		//GameManager.Inst.DBManager.JsonDataHandler.LoadAllItemStats();
		ShipHangarSheet.Initialize();
		ShipInfoSheet.Initialize();
		ShipDataSheet.Initialize();
		EquipmentSheet.Initialize();
		WeaponSheet.Initialize();
		EquipmentInventorySheet.Initialize();
		EquipmentActionSheet.Initialize();
		VaultStorageSheet.Initialize();
	}

	public override void Show ()
	{
		base.Show();

		ResetLoadout();

		GameManager.Inst.CameraController.SetCameraBlur(20f, true);
		Tabs.ForceSelectTab("Ship");

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
			//NGUITools.SetActive(go, false);
			PanelBase sheet = go.GetComponent<PanelBase>();
			sheet.Hide();
		}

		if(tabName == "Ship")
		{
			ShipInfoSheet.Show();
			ShipDataSheet.Show();
			ShipHangarSheet.Show();
			ShipInfoSheet.Refresh();
			ShipDataSheet.Refresh();
			ShipHangarSheet.Refresh();
			SetLayout(3);
		}
		else if(tabName == "Equipment")
		{
			
			EquipmentSheet.Show();
			EquipmentInventorySheet.Show();
			EquipmentActionSheet.Show();
			EquipmentSheet.Refresh();
			EquipmentInventorySheet.Refresh();
			EquipmentInventorySheet.RefreshLoadButtons(null);
			SetLayout(3);

		}
		else if(tabName == "Weapon")
		{
			WeaponSheet.Show();
			EquipmentInventorySheet.Show();
			EquipmentActionSheet.Show();
			WeaponSheet.Refresh();
			EquipmentInventorySheet.Refresh();
			EquipmentInventorySheet.RefreshLoadButtons(null);
			SetLayout(3);
		}
		else if(tabName == "Storage")
		{
			VaultStorageSheet.Show();
			EquipmentActionSheet.Show();
			EquipmentInventorySheet.Show();
			VaultStorageSheet.Refresh();
			EquipmentInventorySheet.Refresh();
			EquipmentInventorySheet.RefreshLoadButtons(null);
			SetLayout(3);
		}
	}

	public void SetLayout(int panes)
	{
		if(panes == 2)
		{
			LeftPane.transform.localPosition = new Vector3(-1 * SidePaneXTwoPane, LeftPane.transform.localPosition.y, LeftPane.transform.localPosition.z);
			RightPane.transform.localPosition = new Vector3(SidePaneXTwoPane, RightPane.transform.localPosition.y, RightPane.transform.localPosition.z);
			LeftPane.transform.localEulerAngles = new Vector3(0, SidePaneAngleTwoPane * -1, 0);
			RightPane.transform.localEulerAngles = new Vector3(0, SidePaneAngleTwoPane, 0);
			NGUITools.SetActive(MiddlePane.gameObject, false);
		}
		else if(panes == 3)
		{
			LeftPane.transform.localPosition = new Vector3(-1 * SidePaneXThreePane, LeftPane.transform.localPosition.y, LeftPane.transform.localPosition.z);
			RightPane.transform.localPosition = new Vector3(SidePaneXThreePane, RightPane.transform.localPosition.y, RightPane.transform.localPosition.z);
			LeftPane.transform.localEulerAngles = new Vector3(0, SidePaneAngleThreePane * -1, 0);
			RightPane.transform.localEulerAngles = new Vector3(0, SidePaneAngleThreePane, 0);
			NGUITools.SetActive(MiddlePane.gameObject, true);
		}
	}

	public void ResetLoadout()
	{
		CurrentLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
		ShipInfoSheet.CurrentLoadout = CurrentLoadout;
		ShipDataSheet.CurrentLoadout = CurrentLoadout;
		EquipmentSheet.CurrentLoadout = CurrentLoadout;
		WeaponSheet.CurrentLoadout = CurrentLoadout;
		EquipmentInventorySheet.CurrentLoadout = CurrentLoadout;
		VaultStorageSheet.CurrentLoadout = CurrentLoadout;
	}



}
