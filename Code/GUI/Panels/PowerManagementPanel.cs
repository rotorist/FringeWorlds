using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManagementPanel : PanelBase
{
	public UISprite PMCursor;
	public Transform ShieldTarget;
	public Transform WeaponTarget;
	public Transform EngineTarget;
	public BarIndicator ShieldPowerInd;
	public BarIndicator WeaponPowerInd;
	public BarIndicator EnginePowerInd;
	public UIButton CenterButton;

	public override void Initialize ()
	{
		base.Initialize();
		PMCursor.transform.localPosition = GameManager.Inst.PlayerControl.PlayerShip.CurrentPowerMgmtButton;
		SetPowerPercentageFromButtonPos(PMCursor.transform.localPosition);
	}

	public override void Hide ()
	{
		base.Hide ();
	}

	public override void Show ()
	{
		base.Show ();
	}



	public void OnMouseHover(UIButton currentButton)
	{
		PMCursor.transform.localPosition = currentButton.transform.localPosition;
		SetPowerPercentageFromButtonPos(PMCursor.transform.localPosition);
		GameManager.Inst.PlayerControl.PlayerShip.CurrentPowerMgmtButton = PMCursor.transform.localPosition;
	}

	public void SetPowerPercentageFromButtonPos(Vector3 buttonLocalPos)
	{
		float shieldDist = Vector3.Distance(buttonLocalPos, ShieldTarget.localPosition);
		float weaponDist = Vector3.Distance(buttonLocalPos, WeaponTarget.localPosition);
		float EngineDist = Vector3.Distance(buttonLocalPos, EngineTarget.localPosition);

		float total = shieldDist + weaponDist + EngineDist;
		Debug.Log(shieldDist + "/" + total);
		float shieldPower = 1 - shieldDist / total;
		float weaponPower = 1 - weaponDist / total;
		float enginePower = 1 - EngineDist / total;

		ShieldPowerInd.SetFillPercentage(shieldPower);
		WeaponPowerInd.SetFillPercentage(weaponPower);
		EnginePowerInd.SetFillPercentage(enginePower);
		//Debug.Log(shieldPower + " " + weaponPower + " " + enginePower);

		GameManager.Inst.PlayerControl.PlayerShip.ShieldPowerAlloc = shieldPower * 1.5f;
		GameManager.Inst.PlayerControl.PlayerShip.WeaponPowerAlloc = weaponPower * 1.5f;
		GameManager.Inst.PlayerControl.PlayerShip.EnginePowerAlloc = enginePower * 1.5f;
	}

	public void OnClick()
	{

	}
}
