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

	public InputState InputState;

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
		/*
		if(GameManager.Inst.SceneType == SceneType.Space || GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			UpdateInSpaceInput();
		}
		else if(GameManager.Inst.SceneType == SceneType.Station)
		{
			UpdateInStationInput();
		}
		*/

		UpdateInput();
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

	private void UpdateInput()
	{
		switch(InputState)
		{
		case InputState.SpaceTest:
			GameManager.Inst.PlayerControl.UpdateSpaceTestInput();
			break;

		case InputState.InFlight:
			GameManager.Inst.PlayerControl.UpdateInFlightKeyInput();
			break;

		case InputState.Autopilot:
			GameManager.Inst.PlayerControl.UpdateAutopilotKeyInput();
			break;

		case InputState.UI:
			GameManager.Inst.PlayerControl.UpdateUIKeyInput();
			break;

		case InputState.DockedUI:
			UpdateInStationInput();
			break;

		case InputState.KeyBindingEnter:

			break;

		}
	}
}

public enum InputState
{
	SpaceTest,
	InFlight,
	Autopilot,
	UI,
	DockedUI,
	KeyBindingEnter,

}