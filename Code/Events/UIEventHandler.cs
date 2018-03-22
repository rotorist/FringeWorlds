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
		
	}

	#endregion

	#region Constructor
	public UIEventHandler()
	{

	}

	#endregion


	public delegate void GeneralUIEventDelegate();



}
