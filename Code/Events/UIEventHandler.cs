using UnityEngine;
using System.Collections;

public class UIEventHandler 
{
	#region Singleton 
	private static UIEventHandler _instance;
	public static UIEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new UIEventHandler();

			return _instance;
		}
	}

	public void OnUnloadScene()
	{
		OnBeginDocking = null;
		OnFadeOutDone = null;
	}

	#endregion

	#region Constructor
	public UIEventHandler()
	{

	}

	#endregion


	public delegate void GeneralUIEventDelegate();
	public static event GeneralUIEventDelegate OnBeginDocking;
	public static event GeneralUIEventDelegate OnFadeOutDone;
	public static event GeneralUIEventDelegate OnWhiteFadeOutDone;
	public static event GeneralUIEventDelegate OnWhiteFadeInDone;

	public static event GeneralUIEventDelegate OnBeginUndocking;
	public static event GeneralUIEventDelegate OnFadeInDone;
	public static event GeneralUIEventDelegate OnCloseStationWindows;
	public static event GeneralUIEventDelegate OnOpenRepairWindow;
	public static event GeneralUIEventDelegate OnOpenStationShipInfo;
	public static event GeneralUIEventDelegate OnOpenTraderPanel;

	public static event GeneralUIEventDelegate OnOpenKeyBindingPanel;
	public static event GeneralUIEventDelegate OnCloseKeyBindingPanel;

	public static event GeneralUIEventDelegate OnOpenPowerManagement;
	public static event GeneralUIEventDelegate OnClosePowerManagement;

	public static event GeneralUIEventDelegate OnOpenEconDebugPanel;
	public static event GeneralUIEventDelegate OnCloseEconDebugPanel;



	public void TriggerBeginDocking()
	{
		if(OnBeginDocking != null)
		{
			OnBeginDocking();
		}
	}

	public void TriggerFadeOutDone()
	{
		if(OnFadeOutDone != null)
		{
			OnFadeOutDone();
		}
	}

	public void TriggerWhiteFadeOutDone()
	{
		if(OnWhiteFadeOutDone != null)
		{
			OnWhiteFadeOutDone();
		}
	}

	public void TriggerWhiteFadeInDone()
	{
		if(OnWhiteFadeInDone != null)
		{
			OnWhiteFadeInDone();
		}
	}

	public void TriggerFadeInDone()
	{
		if(OnFadeInDone != null)
		{
			OnFadeInDone();
		}
	}

	public void TriggerBeginUndocking()
	{
		if(OnBeginUndocking != null)
		{
			OnBeginUndocking();
		}
	}

	public void TriggerCloseStationWindows()
	{
		if(OnCloseStationWindows != null)
		{
			OnCloseStationWindows();
		}

		GameManager.Inst.UIManager.StationHUDPanel.OnCloseWindow();
	}

	public void TriggerOpenRepairWindow()
	{
		if(OnOpenRepairWindow != null)
		{
			OnOpenRepairWindow();
		}
	}

	public void TriggerOpenStationShipInfo()
	{
		if(OnOpenStationShipInfo != null)
		{
			OnOpenStationShipInfo();
		}
	}

	public void TriggerOpenTraderPanel()
	{
		if(OnOpenTraderPanel != null)
		{
			OnOpenTraderPanel();
		}
	}

	public void TriggerOpenKeyBindingPanel()
	{
		if(OnOpenKeyBindingPanel != null)
		{
			OnOpenKeyBindingPanel();
		}
	}

	public void TriggerCloseKeyBindingPanel()
	{
		if(OnCloseKeyBindingPanel != null)
		{
			OnCloseKeyBindingPanel();
		}
	}

	public void TriggerOpenPowerManagement()
	{
		if(OnOpenPowerManagement != null)
		{
			OnOpenPowerManagement();
		}
	}

	public void TriggerClosePowerManagement()
	{
		if(OnClosePowerManagement != null)
		{
			OnClosePowerManagement();
		}
	}

	public void TriggerOpenEconDebugPanel()
	{
		if(OnOpenEconDebugPanel != null)
		{
			OnOpenEconDebugPanel();
		}
	}

	public void TriggerCloseEconDebugPanel()
	{
		if(OnCloseEconDebugPanel != null)
		{
			OnCloseEconDebugPanel();
		}
	}

}
