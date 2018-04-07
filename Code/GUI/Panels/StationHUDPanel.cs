using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;


public class StationHUDPanel : PanelBase
{
	public GameObject TopHUDHolder;

	public override void Initialize ()
	{
		
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

		if(UIButton.current.name == "btnLaunch")
		{
			UIEventHandler.Instance.TriggerBeginUndocking();
		}
	}

	public void OnCloseWindow()
	{
		EnableAllButtons();
	}

	public void EnableAllButtons()
	{
		UIButton [] buttons = TopHUDHolder.transform.GetComponentsInChildren<UIButton>();
		foreach(UIButton button in buttons)
		{
			button.isEnabled = true;
		}
	}
}
