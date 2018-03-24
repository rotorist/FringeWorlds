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

}
