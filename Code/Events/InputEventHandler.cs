using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

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

	private void UpdateKeyBindingInput()
	{
		KeyInput input = new KeyInput();

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			
			GameManager.Inst.UIManager.KeyBindingPanel.OnKeyBindingSet(input);
			return;
		}


		List<KeyCode> controlKeys = new List<KeyCode>()
		{
			/*
			KeyCode.LeftAlt,
			KeyCode.RightAlt,
			KeyCode.AltGr,
			KeyCode.LeftControl,
			KeyCode.RightControl,
			*/
			KeyCode.LeftShift,
			KeyCode.RightShift,
		};

		foreach(KeyCode code in controlKeys)
		{
			if(Input.GetKey(code))
			{
				input.FnKey = code;
			}
			if(Input.GetKeyUp(code))
			{
				input.FnKey = KeyCode.None;
				input.Key = code;
				GameManager.Inst.UIManager.KeyBindingPanel.OnKeyBindingSet(input);
				return;
			}
		}

		foreach(KeyCode code in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKeyDown(code))
			{
				if(!controlKeys.Contains(code))
				{
					input.Key = code;
					GameManager.Inst.UIManager.KeyBindingPanel.OnKeyBindingSet(input);
					return;
				}
			}
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
			UpdateKeyBindingInput();
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