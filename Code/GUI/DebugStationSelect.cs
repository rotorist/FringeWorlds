using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStationSelect : MonoBehaviour 
{
	public string StationID;
	public UILabel StationIDLabel;

	public void OnButtonClick()
	{
		if(GameManager.Inst.UIManager.EconDebugPanel.IsActive)
		{
			GameManager.Inst.UIManager.EconDebugPanel.OnStationSelect(StationID);
		}
	}
}
