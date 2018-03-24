using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationHUDPanel : PanelBase
{
	public GameObject TopHUDHolder;

	public override void Initialize ()
	{
		
	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		IsActive = true;
	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		IsActive = false;
	}

	public override void PerFrameUpdate ()
	{
		
	}

	public void OnTopHUDClick()
	{
		//enable all buttons first
		UIButton [] buttons = TopHUDHolder.transform.GetComponentsInChildren<UIButton>();
		foreach(UIButton button in buttons)
		{
			button.isEnabled = true;
		}

		if(UIButton.current.name == "btnRepair")
		{
			UIButton button = TopHUDHolder.transform.Find("btnRepair").GetComponent<UIButton>();
			button.isEnabled = false;
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

	}
}
