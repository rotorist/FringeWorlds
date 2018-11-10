using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfoPanel : PanelBase
{
	public ShipInfoSheet ShipInfoSheet;
	public ShipDataSheet ShipDataSheet;
	public EquipmentSheet EquipmentSheet;
	public ShipInventorySheet EquipmentInventorySheet;

	public List<GameObject> AllSheets;

	public float SidePaneXTwoPane;
	public float SidePaneXThreePane;

	public GameObject LeftPane;
	public GameObject MiddlePane;
	public GameObject RightPane;

	public UITabSelection Tabs;

	public override void Initialize ()
	{
		base.Initialize();
		//GameManager.Inst.DBManager.JsonDataHandler.LoadAllItemStats();

	}

	public override void Show ()
	{
		base.Show();
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
			NGUITools.SetActive(go, false);
		}

		if(tabName == "Ship")
		{
			NGUITools.SetActive(ShipInfoSheet.gameObject, true);
			NGUITools.SetActive(ShipDataSheet.gameObject, true);
			ShipInfoSheet.Refresh();
			ShipDataSheet.Refresh();
			SetLayout(2);
		}
		else if(tabName == "Equipment")
		{
			
			NGUITools.SetActive(EquipmentSheet.gameObject, true);
			NGUITools.SetActive(EquipmentInventorySheet.gameObject, true);
			EquipmentSheet.Refresh();
			EquipmentInventorySheet.Refresh();
			SetLayout(3);

		}
	}

	public void SetLayout(int panes)
	{
		if(panes == 2)
		{
			LeftPane.transform.localPosition = new Vector3(-1 * SidePaneXTwoPane, LeftPane.transform.localPosition.y, LeftPane.transform.localPosition.z);
			RightPane.transform.localPosition = new Vector3(SidePaneXTwoPane, RightPane.transform.localPosition.y, RightPane.transform.localPosition.z);
			NGUITools.SetActive(MiddlePane.gameObject, false);
		}
		else if(panes == 3)
		{
			LeftPane.transform.localPosition = new Vector3(-1 * SidePaneXThreePane, LeftPane.transform.localPosition.y, LeftPane.transform.localPosition.z);
			RightPane.transform.localPosition = new Vector3(SidePaneXThreePane, RightPane.transform.localPosition.y, RightPane.transform.localPosition.z);
			NGUITools.SetActive(MiddlePane.gameObject, true);
		}
	}





}
