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
			State = new UIStateInFlight(this);
		}
		else if(sceneType == SceneType.Station)
		{
			State = new UIStateInStation(this);
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

public class UIStateInStation : UIStateBase
{
	public UIStateInStation(UIStateMachine sm)
	{
		
	}

	public override void BeginState()
	{

	}

	public override void EndState()
	{

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

		UIEventHandler.OnFadeOutDone -= OnFadeOutDone;
		UIEventHandler.OnFadeOutDone += OnFadeOutDone;
	}

	public override void EndState()
	{
		UIEventHandler.OnFadeOutDone -= OnFadeOutDone;
	}

	public void OnFadeOutDone()
	{
		GameManager.Inst.LoadStationScene();
	}
}