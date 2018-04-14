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
			State = new UIStateUndocking(this);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.lockState = CursorLockMode.None;
			if(GameManager.Inst.PlayerControl.SpawnStationType == StationType.Station)
			{
				GameManager.Inst.UIManager.FadePanel.SetBlackBGAlpha(1f);
				GameManager.Inst.UIManager.FadePanel.FadeIn(0.6f);
			}
			else if(GameManager.Inst.PlayerControl.SpawnStationType == StationType.JumpGate)
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



		//subscribe events
		UIEventHandler.OnBeginDocking -= OnBeginDocking;
		UIEventHandler.OnBeginDocking += OnBeginDocking;


	}

	public override void EndState()
	{
		UIEventHandler.OnBeginDocking -= OnBeginDocking;
	}

	public void OnBeginDocking()
	{
		EndState();
		SM.State = new UIStateDocking(SM);
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
		

		SM.UIManager.HideAllPanels();
		SM.UIManager.StationHUDPanel.Show();

		UIEventHandler.OnOpenRepairWindow -= OnOpenRepairWindow;
		UIEventHandler.OnOpenRepairWindow += OnOpenRepairWindow;
		UIEventHandler.OnBeginUndocking -= OnBeginUndocking;
		UIEventHandler.OnBeginUndocking += OnBeginUndocking;
	}

	public override void EndState()
	{
		UIEventHandler.OnOpenRepairWindow -= OnOpenRepairWindow;
		UIEventHandler.OnBeginUndocking -= OnBeginUndocking;
	}

	public void OnBeginUndocking()
	{
		GameManager.Inst.UIManager.FadePanel.FadeOut(0.4f);
		EndState();
		SM.State = new UIStateUndocking(SM);
	}
		
	public void OnOpenRepairWindow()
	{
		EndState();
		SM.State = new UIStateRepair(SM);
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