using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEventHandler
{

	#region Singleton 
	private static InputEventHandler _instance;
	public static InputEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new InputEventHandler();

			return _instance;
		}
	}

	public void OnUnloadScene()
	{
		
	}

	#endregion

	#region Constructor
	public InputEventHandler()
	{

	}

	#endregion

	public void PerFrameUpdate()
	{
		if(GameManager.Inst.SceneType == SceneType.Space || GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			UpdateInSpaceInput();
		}
		else if(GameManager.Inst.SceneType == SceneType.Station)
		{
			UpdateInStationInput();
		}
	}


	private void UpdateInSpaceInput()
	{

	}

	private void UpdateInStationInput()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			UIEventHandler.Instance.TriggerCloseStationWindows();
		}
	}
}
