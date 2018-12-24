using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;


public class StationHUDPanel : PanelBase
{
	public List<UIButton> AllToggleButtons;
	public GameObject TopHUDHolder;

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

	public override void PerFrameUpdate ()
	{
		
	}

	public void OnTopHUDClick()
	{
		//close all current windows
		UIEventHandler.Instance.TriggerCloseStationWindows();
		//enable all buttons first
		EnableAllButtons();

		if(UIButton.current.name == "btnRepair")
		{
			UIButton button = TopHUDHolder.transform.Find("btnRepair").GetComponent<UIButton>();
			button.isEnabled = false;
			UIEventHandler.Instance.TriggerOpenRepairWindow();
		}

		if(UIButton.current.name == "btnEquipment")
		{
			UIButton button = TopHUDHolder.transform.Find("btnEquipment").GetComponent<UIButton>();
			button.isEnabled = false;
		}

		if(UIButton.current.name == "btnCommodity")
		{
			UIButton button = TopHUDHolder.transform.Find("btnCommodity").GetComponent<UIButton>();
			button.isEnabled = false;
		}

		if(UIButton.current.name == "btnShip")
		{
			UIButton button = TopHUDHolder.transform.Find("btnShip").GetComponent<UIButton>();
			button.isEnabled = false;
			UIEventHandler.Instance.TriggerOpenStationShipInfo();
		}

		if(UIButton.current.name == "btnLaunch")
		{
			UIEventHandler.Instance.TriggerBeginUndocking();
		}

		if(UIButton.current.name == "btnTrade")
		{
			UIEventHandler.Instance.TriggerOpenTraderPanel();
		}

	}

	public void OnCloseWindow()
	{
		EnableAllButtons();

	}

	public void EnableAllButtons()
	{
		
		foreach(UIButton button in AllToggleButtons)
		{
			button.isEnabled = true;
		}
	}
}
