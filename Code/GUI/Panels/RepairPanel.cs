using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class RepairPanel : PanelBase
{


	public override void Initialize ()
	{
		base.Initialize();
	}

	public override void PerFrameUpdate ()
	{
		
	}

	public override void Show ()
	{
		base.Show();
		GameManager.Inst.CameraController.SetCameraBlur(20f, true);
	}

	public override void Hide ()
	{
		base.Hide();
		GameManager.Inst.CameraController.SetCameraBlur(20f, false);
	}
}
