using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateMachine 
{
	public UIStateBase State;
	public UIManager UIManager;

	public void Initialize(SceneType sceneType)
	{
		UIManager = GameManager.Inst.UIManager;
		if(sceneType == SceneType.Space)
		{
			UIManager.HideAllPanels();
			State = new UIStateUndocking(this);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.lockState = CursorLockMode.None;
			if(GameManager.Inst.PlayerProgress.SpawnStationType == StationType.Station)
			{
				GameManager.Inst.UIManager.FadePanel.SetBlackBGAlpha(1f);
				GameManager.Inst.UIManager.FadePanel.FadeIn(0.6f);
			}
			else if(GameManager.Inst.PlayerProgress.SpawnStationType == StationType.JumpGate)
			{
				GameManager.Inst.UIManager.FadePanel.SetWhiteBGAlpha(1f);
				GameManager.Inst.UIManager.FadePanel.WhiteFadeIn(0.3f);
			}
		}
		else if(sceneType == SceneType.Station)
		{
			State = new UIStateDocking(this);
			GameManager.Inst.UIManager.FadePanel.FadeIn(0.6f);
			GameManager.Inst.CameraController.SetCameraBlur(1000, false);
		}
	}

}

public abstract class UIStateBase
{
	public UIStateMachine SM;
	public string Name;
	public abstract void BeginState();
	public abstract void EndState();

	public virtual void UpdateState()
	{

	}
}

public class UIStateInFlight : UIStateBase
{

	public UIStateInFlight(UIStateMachine sm)
	{
		Name = "UIStateInFlight";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();

		//GameManager.Inst.PlayerControl.PauseGame(true);

		InputEventHandler.Instance.InputState = InputState.InFlight;

		//subscribe events
		UIEventHandler.OnBeginDocking -= OnBeginDocking;
		UIEventHandler.OnBeginDocking += OnBeginDocking;
		UIEventHandler.OnOpenKeyBindingPanel -= OnOpenKeyBindingPanel;
		UIEventHandler.OnOpenKeyBindingPanel += OnOpenKeyBindingPanel;
		UIEventHandler.OnOpenPowerManagement -= OnOpenPowerManagement;
		UIEventHandler.OnOpenPowerManagement += OnOpenPowerManagement;
		UIEventHandler.OnOpenEconDebugPanel -= OnOpenEconDebugPanel;
		UIEventHandler.OnOpenEconDebugPanel += OnOpenEconDebugPanel;
	}

	public override void EndState()
	{
		UIEventHandler.OnBeginDocking -= OnBeginDocking;
		UIEventHandler.OnOpenKeyBindingPanel -= OnOpenKeyBindingPanel;
		UIEventHandler.OnOpenPowerManagement -= OnOpenPowerManagement;
		UIEventHandler.OnOpenEconDebugPanel -= OnOpenEconDebugPanel;
	}

	public void OnBeginDocking()
	{
		EndState();
		SM.State = new UIStateDocking(SM);
	}

	public void OnOpenKeyBindingPanel()
	{
		EndState();
		SM.State = new UIStateKeyBinding(SM);
	}

	public void OnOpenPowerManagement()
	{
		EndState();
		SM.State = new UIStatePowerManagement(SM);
	}

	public void OnOpenEconDebugPanel()
	{
		EndState();
		SM.State = new UIStateEconDebug(SM);
	}
}



public class UIStateDocking : UIStateBase
{
	public UIStateDocking(UIStateMachine sm)
	{
		Name = "UIStateDocking";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.FadePanel.Show();
		Debug.Log("Begin UIStateDocking");

		InputEventHandler.Instance.InputState = InputState.None;

		if(GameManager.Inst.SceneType == SceneType.Space || GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			UIEventHandler.OnFadeOutDone -= OnFadeOutDone;
			UIEventHandler.OnFadeOutDone += OnFadeOutDone;
			UIEventHandler.OnWhiteFadeOutDone -= OnWhiteFadeOutDone;
			UIEventHandler.OnWhiteFadeOutDone += OnWhiteFadeOutDone;
		}
		else if(GameManager.Inst.SceneType == SceneType.Station)
		{
			UIEventHandler.OnFadeInDone -= OnFadeInDone;
			UIEventHandler.OnFadeInDone += OnFadeInDone;
		}


	}

	public override void EndState()
	{
		UIEventHandler.OnFadeInDone -= OnFadeInDone;
		UIEventHandler.OnFadeOutDone -= OnFadeOutDone;
		UIEventHandler.OnWhiteFadeOutDone -= OnWhiteFadeOutDone;
	}

	public void OnFadeOutDone()
	{
		GameManager.Inst.LoadStationScene();
	}

	public void OnFadeInDone()
	{
		EndState();
		SM.State = new UIStateInStation(SM);
	}

	public void OnWhiteFadeOutDone()
	{
		GameManager.Inst.LoadSpaceScene();
	}


}

public class UIStateUndocking : UIStateBase
{
	public UIStateUndocking(UIStateMachine sm)
	{
		Name = "UIStateUndocking";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.FadePanel.Show();

		InputEventHandler.Instance.InputState = InputState.None;

		if(GameManager.Inst.SceneType == SceneType.Station)
		{
			UIEventHandler.OnFadeOutDone -= OnFadeOutDone;
			UIEventHandler.OnFadeOutDone += OnFadeOutDone;
		}
		else if(GameManager.Inst.SceneType == SceneType.Space || GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			UIEventHandler.OnFadeInDone -= OnFadeInDone;
			UIEventHandler.OnFadeInDone += OnFadeInDone;
			UIEventHandler.OnWhiteFadeInDone -= OnWhiteFadeInDone;
			UIEventHandler.OnWhiteFadeInDone += OnWhiteFadeInDone;
		}


	}

	public override void EndState()
	{
		UIEventHandler.OnFadeInDone -= OnFadeInDone;
		UIEventHandler.OnFadeOutDone -= OnFadeOutDone;
		UIEventHandler.OnWhiteFadeInDone -= OnWhiteFadeInDone;
	}

	public void OnFadeOutDone()
	{
		GameManager.Inst.LoadSpaceScene();
	}

	public void OnFadeInDone()
	{
		EndState();
		SM.State = new UIStateInFlight(SM);
	}

	public void OnWhiteFadeInDone()
	{
		EndState();
		SM.State = new UIStateInFlight(SM);
	}
}


public class UIStatePowerManagement : UIStateBase
{
	public UIStatePowerManagement(UIStateMachine sm)
	{
		Name = "UIStatePowerManagement";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.PowerManagementPanel.Show();

		InputEventHandler.Instance.InputState = InputState.PowerManagement;
		GameManager.Inst.PlayerControl.SetMouseFlight(false);

		UIEventHandler.OnClosePowerManagement -= OnClosePowerManagement;
		UIEventHandler.OnClosePowerManagement += OnClosePowerManagement;
	}

	public override void EndState ()
	{
		GameManager.Inst.PlayerControl.SetMouseFlight(true);
		UIEventHandler.OnClosePowerManagement -= OnClosePowerManagement;
	}

	public void OnClosePowerManagement()
	{
		EndState();
		SM.State = new UIStateInFlight(SM);
	}
}


public class UIStateKeyBinding : UIStateBase
{
	public UIStateKeyBinding(UIStateMachine sm)
	{
		Name = "UIStateKeyBinding";
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		SM.UIManager.HideAllPanels();
		SM.UIManager.KeyBindingPanel.Show();
		SM.UIManager.FadePanel.Show();

		SM.UIManager.FadePanel.BlackBG.alpha = 0.7f;

		InputEventHandler.Instance.InputState = InputState.UI;

		UIEventHandler.OnCloseKeyBindingPanel -= OnCloseKeyBindingPanel;
		UIEventHandler.OnCloseKeyBindingPanel += OnCloseKeyBindingPanel;
	}

	public override void EndState ()
	{
		SM.UIManager.FadePanel.BlackBG.alpha = 0;
		UIEventHandler.OnCloseKeyBindingPanel -= OnCloseKeyBindingPanel;
	}

	public void OnCloseKeyBindingPanel()
	{
		EndState();
		SM.State = new UIStateInFlight(SM);
	}
}

public class UIStateEconDebug : UIStateBase
{
	public UIStateEconDebug(UIStateMachine sm)
	{
		Name = "UIStateEconDebug";
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		SM.UIManager.HideAllPanels();
		SM.UIManager.EconDebugPanel.Show();
		SM.UIManager.FadePanel.Show();

		SM.UIManager.FadePanel.BlackBG.alpha = 0.7f;

		InputEventHandler.Instance.InputState = InputState.UI;

		UIEventHandler.OnCloseEconDebugPanel -= OnCloseEconDebugPanel;
		UIEventHandler.OnCloseEconDebugPanel += OnCloseEconDebugPanel;
	}

	public override void EndState ()
	{
		SM.UIManager.FadePanel.BlackBG.alpha = 0;
		UIEventHandler.OnCloseEconDebugPanel -= OnCloseEconDebugPanel;
	}

	public void OnCloseEconDebugPanel()
	{
		EndState();
		SM.State = new UIStateInFlight(SM);
	}
}


public class UIStateInStation : UIStateBase
{
	public UIStateInStation(UIStateMachine sm)
	{
		Name = "UIStateInStation";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		Debug.Log("Begin UIStateInStation");

		SM.UIManager.HideAllPanels();
		SM.UIManager.StationHUDPanel.Show();

		InputEventHandler.Instance.InputState = InputState.DockedUI;

		UIEventHandler.OnOpenRepairWindow -= OnOpenRepairWindow;
		UIEventHandler.OnOpenRepairWindow += OnOpenRepairWindow;
		UIEventHandler.OnOpenStationShipInfo -= OnOpenStationShipInfo;
		UIEventHandler.OnOpenStationShipInfo += OnOpenStationShipInfo;
		UIEventHandler.OnOpenTraderPanel -= OnOpenTraderPanel;
		UIEventHandler.OnOpenTraderPanel += OnOpenTraderPanel;

		UIEventHandler.OnBeginUndocking -= OnBeginUndocking;
		UIEventHandler.OnBeginUndocking += OnBeginUndocking;


	}

	public override void EndState()
	{
		UIEventHandler.OnOpenRepairWindow -= OnOpenRepairWindow;
		UIEventHandler.OnBeginUndocking -= OnBeginUndocking;
		UIEventHandler.OnOpenStationShipInfo -= OnOpenStationShipInfo;
		UIEventHandler.OnOpenTraderPanel -= OnOpenTraderPanel;
	}

	public void OnBeginUndocking()
	{
		GameManager.Inst.SaveGameManager.CreateSaveInStation();
		GameManager.Inst.UIManager.FadePanel.FadeOut(0.4f);
		EndState();
		SM.State = new UIStateUndocking(SM);
	}
		
	public void OnOpenRepairWindow()
	{
		EndState();
		SM.State = new UIStateRepair(SM);
	}

	public void OnOpenStationShipInfo()
	{
		EndState();
		SM.State = new UIStateStationShipInfo(SM);
	}

	public void OnOpenTraderPanel()
	{
		EndState();
		SM.State = new UIStateTraderPanel(SM);
	}
}


public class UIStateRepair : UIStateBase
{
	public UIStateRepair(UIStateMachine sm)
	{
		Name = "UIStateRepair";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		SM.UIManager.HideAllPanels();
		SM.UIManager.StationHUDPanel.Show();
		SM.UIManager.RepairPanel.Show();
		SM.UIManager.ErrorMessagePanel.Show();

		InputEventHandler.Instance.InputState = InputState.DockedUI;

		UIEventHandler.OnCloseStationWindows -= OnCloseWindow;
		UIEventHandler.OnCloseStationWindows += OnCloseWindow;
	}

	public override void EndState()
	{
		UIEventHandler.OnCloseStationWindows -= OnCloseWindow;
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateInStation(SM);
	}
}

public class UIStateStationShipInfo : UIStateBase
{
	public UIStateStationShipInfo(UIStateMachine sm)
	{
		Name = "UIStateStationShipInfo";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		//SM.UIManager.ShipInfoPanel.CurrentLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
		
		SM.UIManager.HideAllPanels();
		SM.UIManager.StationHUDPanel.Show();
		SM.UIManager.ShipInfoPanel.Show();
		SM.UIManager.ErrorMessagePanel.Show();

		InputEventHandler.Instance.InputState = InputState.DockedUI;

		UIEventHandler.OnCloseStationWindows -= OnCloseWindow;
		UIEventHandler.OnCloseStationWindows += OnCloseWindow;
	}

	public override void EndState()
	{
		UIEventHandler.OnCloseStationWindows -= OnCloseWindow;
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateInStation(SM);
	}
}

public class UIStateTraderPanel : UIStateBase
{
	public UIStateTraderPanel(UIStateMachine sm)
	{
		Name = "UIStateTraderPanel";
		SM = sm;
		BeginState();
	}

	public override void BeginState()
	{
		SM.UIManager.HideAllPanels();
		SM.UIManager.TraderPanel.Show();
		SM.UIManager.StationHUDPanel.Show();
		SM.UIManager.ErrorMessagePanel.Show();

		InputEventHandler.Instance.InputState = InputState.DockedUI;

		UIEventHandler.OnCloseStationWindows -= OnCloseWindow;
		UIEventHandler.OnCloseStationWindows += OnCloseWindow;
	}

	public override void EndState()
	{
		UIEventHandler.OnCloseStationWindows -= OnCloseWindow;
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateInStation(SM);
	}
}