using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventHandler
{

	#region Singleton 
	private static GameEventHandler _instance;
	public static GameEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new GameEventHandler();

			return _instance;
		}
	}

	public void OnUnloadScene()
	{

	}

	#endregion

	#region Constructor
	public GameEventHandler()
	{

	}

	#endregion

	#region Delegates

	public delegate void ShipEventDelegate(ShipBase ship);
	public static event ShipEventDelegate OnShipDeath;

	#endregion


	#region Triggers
	public void TriggerShipDeath(ShipBase ship)
	{
		if(OnShipDeath != null)
		{
			OnShipDeath(ship);
		}
	}

	#endregion



}
